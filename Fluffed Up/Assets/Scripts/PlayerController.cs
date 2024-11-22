using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Xml;

public class PlayerController : CharacterClass
{
    // Player attributes
    public UnityEvent<float, int> AttackEvent;
    public UnityEvent<float> DamageEvent;
    [Header("Shooting")]
    public GameObject projectilePrefab;       // The projectile prefab to instantiate
    public Transform projectileSpawnPoint;    // Where the projectile will spawn
    public float projectileDamage;
    public AudioClip attackSound;
    public AudioClip shootingSound;
    Vector3 dodgeDir = Vector3.zero;
    public AudioClip reloadSound;

    #region Coin Attributes
    private int coins;
    private int coinFlushCounter;
    bool coinsAreFlushing;
    float timeSinceLastCoinChange;
    float coinFlushWaitTime;
    #endregion

    #region Shooting Attributes
    int maxAmmo = 13;
    int currAmmo;
    float reloadTime = 2f;
    float shotTime = 0.2f;
    bool isReloading;
    Dictionary<int, Image> ammoIndicators;
    #endregion

    [System.Serializable]
    public class ItemProperties
    {
        public float totalAmount;
        public float quantity;
        public float value;

        public ItemProperties(float newQuantity, float newValue)
        {
            quantity = newQuantity;
            value = newValue;
            totalAmount = quantity * value;
        }
    }
    private Dictionary<string, ItemProperties> inventory = new Dictionary<string, ItemProperties>();

    [Header("Melee attack attibutes")]
    private float attackComboCooldown;
    private int attackComboMax;
    public int CurrentAttackCounter
    {
        get => currentAttackCounter;
        private set => currentAttackCounter = value >= attackComboMax ? 0 : value;
    }
    public int currentAttackCounter;
    public bool ATTACK_1_BOOL;
    public bool ATTACK_2_BOOL;
    public int enemyStunDelayMilli;

    [Header("Inputs")]
    [SerializeField]
    private InputMap inputs;

    [Header("Camera")]
    [SerializeField]
    public Transform cameraTransform;

    [Header("UI")]
    [SerializeField]
    public TextMeshProUGUI coinCounterText; // For Unity UI Text
    public TextMeshProUGUI coinFlushCounterText; // For Unity UI Text
    public TextMeshProUGUI healthCounterText; // For Unity UI Text
    public TextMeshProUGUI attackPowerText; // For Unity UI Text
    public TextMeshProUGUI attackSpeedText; // For Unity UI Text
    public TextMeshProUGUI defenseText; // For Unity UI Text
    public TextMeshProUGUI moveSpeedText; // For Unity UI Text

    private void Awake()
    {
        groundMask = ~(1 << LayerMask.GetMask("Ground"));
        inputs = GetComponent<InputMap>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ResetJump();
    }

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 10f;
        rotationSpeed = 360f;
        jumpForce = 5f;
        jumpTime = 3f;
        jumpCooldown = 0.5f;
        airSpeedMultiplier = 1f;
        attackPower = 70f;
        health = 100f;
        maxHealth = 100f;
        CurrentAttackCounter = 0;
        attackComboMax = 3;
        attackComboCooldown = 1f;
        attackSpeed = 1f;
        coinFlushWaitTime = 2f;
        isDodging = false;
        invincibilityFrames = 10;
        currInvincibilityFrames = 0;
        currAmmo = maxAmmo;
        isReloading = false;
        projectileDamage = 30f;
        attackDelayInMilli = 300; 

        if (SelectChar.characterID == 1) // If the shooter character is selected
        {
            ammoIndicators = new Dictionary<int, Image>()
            {
                { 1, GameObject.Find("Ammo1").GetComponent<Image>() },
                { 2, GameObject.Find("Ammo2").GetComponent<Image>() },
                { 3, GameObject.Find("Ammo3").GetComponent<Image>() },
                { 4, GameObject.Find("Ammo4").GetComponent<Image>() },
                { 5, GameObject.Find("Ammo5").GetComponent<Image>() },
                { 6, GameObject.Find("Ammo6").GetComponent<Image>() },
                { 7, GameObject.Find("Ammo7").GetComponent<Image>() },
                { 8, GameObject.Find("Ammo8").GetComponent<Image>() },
                { 9, GameObject.Find("Ammo9").GetComponent<Image>() },
                { 10, GameObject.Find("Ammo10").GetComponent<Image>() },
                { 11, GameObject.Find("Ammo11").GetComponent<Image>() },
                { 12, GameObject.Find("Ammo12").GetComponent<Image>() },
                { 13, GameObject.Find("Ammo13").GetComponent<Image>() },
            };
        }

        // Coin stuff
        coins = 0;
        coinFlushCounter = 0;
        timeSinceLastCoinChange = Time.time;
        coinsAreFlushing = false;

        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
        else
            Debug.Log("No HealthBar attached to PlayerController");

        animator.SetFloat("attackSpeed", attackSpeed);

        UpdateAllCounters();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        #region Grounded Check
        isGrounded = Physics.CheckSphere(rb.transform.position, 0.2f, groundMask);
        #endregion

        #region Movement Control

        Vector3 moveDir = GetCameraRelativeMovement(horizontal, vertical);

        Vector3 move = isDodging ? dodgeDir * 1.3f : new Vector3(moveDir.x, 0f, moveDir.z);

        if (isGrounded)
            rb.MovePosition(transform.position + move * moveSpeed * Time.deltaTime);
        else
            rb.MovePosition(transform.position + move * moveSpeed * airSpeedMultiplier * Time.deltaTime);

        isRunning = move != Vector3.zero && isGrounded && !isDodging;

        #endregion

        #region Rotate player with camera
        if (!isDodging) {
            Vector3 viewDirection = transform.position - new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
            transform.forward = viewDirection.normalized;
        }
        #endregion

        #region Coin Flush Handling
        if (Time.time - timeSinceLastCoinChange > coinFlushWaitTime && coinFlushCounter != 0)
            coinsAreFlushing = true;

        if (coinsAreFlushing)
        {
            if (coinFlushCounter < 0)
            {
                coinFlushCounter += 1;
                coins -= 1;
            }
            else if (coinFlushCounter > 0)
            {
                coinFlushCounter -= 1;
                coins += 1;
            }

            UpdateCoinCounter();
        }

        if (coinFlushCounter == 0)
            coinsAreFlushing = false;
        #endregion

        #region Stat UI
        attackPowerText.text = attackPower.ToString();
        attackSpeedText.text = attackSpeed.ToString();
        defenseText.text = defense.ToString();
        moveSpeedText.text = moveSpeed.ToString();
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isRunning", isRunning);
        if (currAmmo < 1 && !isReloading)
        {
            PlaySoundEffect(reloadSound);
            StartCoroutine(WaitToReload(reloadTime));
        }

        if (SelectChar.characterID == 1) // If the shooter character is selected
        {
            if (Input.GetMouseButtonDown(0) && !isAttacking && !isReloading && currAmmo > 0)
            {
                Debug.Log("Left mouse button clicked - calling Shoot() for shooter character");
                StartCoroutine(WaitToChamber(shotTime));
                Shoot();
            }
        }
        else // If the sword character is selected
        {
            if (Input.GetMouseButtonDown(0) && !isAttacking)
            {
                Debug.Log("Left mouse button clicked - calling meleeAttack() for sword character");
                meleeAttack();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dodge();
        }

        if (inputs.jump && isGrounded && !isJumping && !isDodging)
        {
            isJumping = true;
            Jump(1f);
        }
        healthBar.SetHealth(health);

        if (Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking)
        {
            HealSelf();
        }

        if (Input.GetKeyDown(KeyCode.R) && currAmmo < maxAmmo)
        {
            PlaySoundEffect(reloadSound);
            StartCoroutine(WaitToReload(reloadTime));
        }

        if (currInvincibilityFrames > 0)
            currInvincibilityFrames--;
    }

    public Vector3 GetCameraRelativeMovement(float horizontal, float vertical)
    {
        Vector3 camRight = cameraTransform.right;

        camRight.y = 0;

        Vector3 rightRelataive = horizontal * camRight;

        return GetCameraForwardRelative(vertical) + rightRelataive;
    }

    public Vector3 GetCameraForwardRelative(float vertical)
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        return vertical * camForward;
    }

    public void Dodge()
    {
        if (dodgeSkill != null)
            if (dodgeSkill.UseSkill())
            {
                isDodging = true;
                StartCoroutine(ResetDodgeState());
                if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
                {
                    dodgeDir = transform.forward;
                }
                else
                {
                    dodgeDir = Vector3.Normalize(GetCameraRelativeMovement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
                    transform.forward = dodgeDir.normalized;
                }
                animator.SetTrigger("dodge");
            }
    }

    private IEnumerator ResetDodgeState()
    {
        yield return new WaitForSeconds((animator.GetCurrentAnimatorStateInfo(0).length));
        isDodging = false;
        Debug.Log("Dodge reset.");
    }

    void Shoot()
    {
        Debug.Log("Shoot() method is being called");

        PlaySoundEffect(shootingSound);

        // Make projectile shoot towards camera's forward direction
        Vector3 shootDirection = cameraTransform.forward;

        // If you want to shoot slightly above the camera's forward direction, you can add cameraTransform.up
        shootDirection += cameraTransform.up * 0.115f;  // Slightly shoot upward
        shootDirection -= cameraTransform.right * 0.025f;  // This slightly shoots the projectile to the right or left if needed

        // Normalize the shoot direction to ensure consistent projectile speed
        shootDirection.Normalize();

        // Instantiate the projectile at the spawn point
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        // Ignore collision between the projectile and the player
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());
        projectile.GetComponent<Projectile>().SetDamage(projectileDamage);

        // Set the projectile's direction based on the adjusted shoot direction
        projectile.transform.forward = shootDirection;

        // Apply velocity to the projectile using the Rigidbody component
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * projectile.GetComponent<Projectile>().GetSpeed(); //Set velocity with direction and speed

            // apply gravity
            rb.useGravity = true;
        }

        ammoIndicators[currAmmo].canvasRenderer.SetAlpha(0.2f);
        currAmmo -= 1;
    }

    public IEnumerator WaitToReload(float duration)
    {
        isReloading = true;
        yield return new WaitForSeconds(duration);
        foreach (KeyValuePair<int, Image> ammo in ammoIndicators)
        {
            ammo.Value.canvasRenderer.SetAlpha(1f);
        }
        isReloading = false;
        currAmmo = maxAmmo;
    }

    public IEnumerator WaitToChamber(float duration)
    {
        isAttacking = true;
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    void meleeAttack()
    {
        isAttacking = true;
        switch (CurrentAttackCounter)
        {
            case 0:
                animator.SetTrigger("attack01");
                break;
            case 1:
                animator.SetTrigger("attack02");
                break;
            default:
                break;
        }
        CurrentAttackCounter = (CurrentAttackCounter + 1) % 2;

        // Reset the attacking state after the attack animation finishes
        StartCoroutine(ResetAttackState());

        // Play attack sound
        PlaySoundEffect(attackSound);

        // Find nearby enemies
        AttackEvent?.Invoke(attackPower, attackDelayInMilli);
    }


    public void CollectItem(CollectibleItem item)
    {
        if (!inventory.ContainsKey(item.itemName))
        {
            // Debug.LogWarning("Adding new item to inventory");
            ItemProperties properties = new ItemProperties(item.quantity, item.value);
            inventory.Add(item.itemName, properties);
        }
        else
        {
            ItemProperties properties = inventory[item.itemName];
            properties.quantity += item.quantity;
            properties.totalAmount += item.totalAmount;
            inventory[item.itemName] = properties;
            // Debug.LogWarning("Item already exists in inventory. Updating amount");
        }

        // Debug.Log($"Collected: {item.itemName}");
        UpdateAllCounters();
    }

    public ItemProperties GetItemProperties(string key)
    {
        if (inventory.TryGetValue(key, out ItemProperties properties))
        {
            // Debug.LogWarning("Item found in inventory.");
            return properties;
        }
        else
        {
            // Debug.LogWarning("Item not found in inventory.");
            return null;
        }
    }

    void UpdateAllCounters()
    {
        UpdateCoinCounter();
        UpdateHealthPackCounter();
    }

    public override void TakeDamage(float damage, int additionalDelay)
    {
        base.TakeDamage(damage, additionalDelay);

        animator.Play("GetHit");
    }

    public void UpdateCoinCounter()
    {
        coinCounterText.text = coins.ToString();

        if (coinFlushCounter == 0)
        {
            coinFlushCounterText.text = "";
        }
        else if (coinFlushCounter > 0)
        {
            coinFlushCounterText.text = "+$" + coinFlushCounter.ToString();
            coinFlushCounterText.color = Color.green;
        }
        else
        {
            coinFlushCounterText.text = "-$" + Math.Abs(coinFlushCounter).ToString();
            coinFlushCounterText.color = Color.red;
        }
    }

    void RemoveCoins(float amount)
    {
        ItemProperties properties = GetItemProperties("Coin");
        if(properties != null)
        {
            if(properties.totalAmount != 0)
            {
                properties.totalAmount -= amount;
                if (properties.totalAmount < 0) {
                    properties.totalAmount = 0;
                }
                inventory["Coin"] = properties;
                Debug.Log("Removing coins");
                Debug.Log(inventory["Coin"].quantity);
                UpdateCoinCounter();
            }
            else
            {
                Debug.Log("No coins to remove.");
            }
        }
        else
        {
            Debug.Log("No coins to remove.");
        }
    }

    void UpdateHealthPackCounter()
    {
        ItemProperties properties = GetItemProperties("Health");
        if(properties != null)
        {
            healthCounterText.text = properties.quantity.ToString();
        }
        else
        {
            healthCounterText.text = "0";
        }
    }

    void HealSelf()
    {
        ItemProperties properties = GetItemProperties("Health");
        if(properties != null)
        {
            if(properties.quantity != 0 && health < maxHealth)
            {
                Heal(properties.value);
                properties.totalAmount -= properties.quantity * properties.value;
                if (properties.totalAmount < 0) {
                    properties.totalAmount = 0;
                }
                properties.quantity--;
                inventory["Health"] = properties;
                Debug.Log("Healed");
                UpdateHealthPackCounter();
            }
            else
            {
                Debug.Log("No health to heal.");
            }
        }
        else
        {
            Debug.Log("No health to heal.");
        }
    }

    public void UpdateCoins(int addedCoins)
    {
        if (coinsAreFlushing)
        {
            coinsAreFlushing = false;
            coins += coinFlushCounter;
            coinFlushCounter = 0;
        }
        else if (coinFlushCounter * addedCoins < 0)
        {
            coinsAreFlushing = false;
            coins += coinFlushCounter;
            coinFlushCounter = addedCoins;
        }
        else
        {
            coinFlushCounter += addedCoins;
            timeSinceLastCoinChange = Time.time;
        }

        UpdateCoinCounter();
    }

    public int GetCoins()
    {
        return coins;
    }
}
