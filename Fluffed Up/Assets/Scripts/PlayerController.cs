using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : CharacterClass
{
    // Player attributes
    public UnityEvent<float, int> AttackEvent;
    public UnityEvent<float> DamageEvent;

    [Header("Shooting")]
    public GameObject projectilePrefab;       // The projectile prefab to instantiate
    public Transform projectileSpawnPoint;    // Where the projectile will spawn
    public float projectileDamage;
    private float projectileSpeed = 300f;
    private LayerMask aimLayerMask; // Invertered layer mask
    #region Coin Attributes
    public bool isShopping;
    private int coins;
    private int coinFlushCounter;
    bool coinsAreFlushing;
    float timeSinceLastCoinChange;
    float coinFlushWaitTime;
    #endregion

    #region Movement
    float horizontal;
    float vertical;
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
    public UpgradeFlags upgradeFlags;

    [Header("Sound Effects for PlayerController")]
    [SerializeField]
    public AudioClip attackSound;
    public AudioClip shootingSound;
    public AudioClip reloadSound;
    public AudioClip itemPickupSound;
    public AudioClip slayHitSound;

    [Header("Melee attack attibutes")]
    [SerializeField]
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
    private bool canMove = true;

    [Header("Camera")]
    [SerializeField]
    public Transform cameraTransform;

    [Header("UI")]
    [SerializeField]
    public TextMeshProUGUI coinCounterText;
    public TextMeshProUGUI coinFlushCounterText;
    public TextMeshProUGUI healthCounterText;
    public TextMeshProUGUI attackPowerText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI reloadSpeedText;
    public TextMeshProUGUI bulletChamberSpeed; 
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI moveSpeedText;

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
        coinFlushWaitTime = 1f;
        isDodging = false;
        currInvincibilityFrames = 0;
        currAmmo = maxAmmo;
        isReloading = false;
        projectileDamage = 30f;
        attackDelayInMilli = 300;

        upgradeFlags = FindObjectOfType<UpgradeFlags>();
        Debug.Log($"PlayerController found UpgradeFlags: {upgradeFlags}");

        if (SelectChar.characterID == 1) // If the shooter character is selected
        {
            ammoIndicators = new Dictionary<int, UnityEngine.UI.Image>()
            {
                { 1, GameObject.Find("Ammo1").GetComponent<UnityEngine.UI.Image>() },
                { 2, GameObject.Find("Ammo2").GetComponent<UnityEngine.UI.Image>() },
                { 3, GameObject.Find("Ammo3").GetComponent<UnityEngine.UI.Image>() },
                { 4, GameObject.Find("Ammo4").GetComponent<UnityEngine.UI.Image>() },
                { 5, GameObject.Find("Ammo5").GetComponent<UnityEngine.UI.Image>() },
                { 6, GameObject.Find("Ammo6").GetComponent<UnityEngine.UI.Image>() },
                { 7, GameObject.Find("Ammo7").GetComponent<UnityEngine.UI.Image>() },
                { 8, GameObject.Find("Ammo8").GetComponent<UnityEngine.UI.Image>() },
                { 9, GameObject.Find("Ammo9").GetComponent<UnityEngine.UI.Image>() },
                { 10, GameObject.Find("Ammo10").GetComponent<UnityEngine.UI.Image>() },
                { 11, GameObject.Find("Ammo11").GetComponent<UnityEngine.UI.Image>() },
                { 12, GameObject.Find("Ammo12").GetComponent<UnityEngine.UI.Image>() },
                { 13, GameObject.Find("Ammo13").GetComponent<UnityEngine.UI.Image>() },
            };
        }

        cameraTransform = GameObject.Find("MainCamera").transform;
        projectileSpawnPoint = GameObject.Find("ProjectileSpawnPoint").transform;
        aimLayerMask = LayerMask.GetMask("Player", "MiniMap");

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
        #region Grounded Check
        isGrounded = Physics.CheckSphere(rb.transform.position, 0.2f, groundMask);
        #endregion

        #region Coin Flush Handling
        if (isShopping && coinFlushCounter != 0)
        {
            UpdateCoins(0);
        }

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
            else
            {
                coinsAreFlushing = false;
            }
            UpdateCoinCounter();
        }


        #endregion

        #region Movement Control
        if (canMove)
        {
            Vector3 moveDir;
            if (isDodging)
            {
                moveDir = GetDodgeDirectionAndRotate();
                Vector3 newVelocity = new Vector3(moveDir.x, 0, moveDir.z) * moveSpeed * dodgeSkill.GetSkillValue();
                newVelocity.y = rb.velocity.y;
                rb.velocity = newVelocity;
            }
            else
            {
                moveDir = GetCameraRelativeMovement(horizontal, vertical);
                Vector3 newVelocity = new Vector3(moveDir.x, 0, moveDir.z) * moveSpeed;
                if (!isGrounded)
                {
                    newVelocity *= airSpeedMultiplier;
                }
                newVelocity.y = rb.velocity.y;
                rb.velocity = newVelocity;

            }

            isRunning = moveDir != Vector3.zero && isGrounded && !isDodging;
        }

        #endregion

        #region Rotate player with camera
        if (!isDodging)
        {
            Vector3 viewDirection = transform.position - new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
            transform.forward = viewDirection.normalized;
        }
        #endregion

        #region Stat UI
        attackPowerText.text = attackPower.ToString();
        defenseText.text = defense.ToString();
        moveSpeedText.text = moveSpeed.ToString();

        if (SelectChar.characterID == 1)
        {
            reloadSpeedText.text = reloadTime.ToString();
            bulletChamberSpeed.text = shotTime.ToString();
        } 
        else if (SelectChar.characterID == 0)
        {
            attackSpeedText.text = attackSpeed.ToString();
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        animator.SetBool("isRunning", isRunning);
        if (currAmmo < 1 && !isReloading)
        {
            PlaySoundEffect(reloadSound);
            StartCoroutine(WaitToReload(reloadTime));
        }

        if (SelectChar.characterID == 1) // If the shooter character is selected
        {
            if (Input.GetMouseButton(0) && !isAttacking && !isReloading && currAmmo > 0 && !isDodging)
            {
                Debug.Log("Left mouse button clicked - calling Shoot() for shooter character");
                StartCoroutine(WaitToChamber(shotTime));
                Shoot();
            }
        }
        else // If the sword character is selected
        {
            if (Input.GetMouseButton(0) && !isAttacking && !isDodging)
            {
                Debug.Log("Left mouse button clicked - calling meleeAttack() for sword character");
                meleeAttack();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dodgeSkill != null)
        {
            dodgeSkill.UseSkill();
        }

        if (inputs.jump && isGrounded && !isJumping && !isDodging)
        {
            isJumping = true;
            Jump();
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
        {
            currInvincibilityFrames--;
        }

        if (health <= 0) {
            Die();
        }
    }

    public Vector3 GetCameraRelativeMovement(float horizontal, float vertical)
    {
        Vector3 camRight = cameraTransform.right;

        camRight.y = 0;

        Vector3 rightRelataive = horizontal * camRight;

        return Vector3.Normalize(GetCameraForwardRelative(vertical) + rightRelataive);
    }

    public Vector3 GetCameraForwardRelative(float vertical)
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        return vertical * camForward;
    }

    private Vector3 GetDodgeDirectionAndRotate()
    {
        if (horizontal == 0 && vertical == 0)
        {
            return transform.forward;
        }
        else
        {
            Vector3 dir = Vector3.Normalize(GetCameraRelativeMovement(horizontal, vertical));
            transform.forward = dir;
            return dir.normalized;
        }
    }

    public void ResetDodgeState(float resetTime = 0f)
    {
        StartCoroutine(ResetDodgeStateCoroutine(resetTime));
    }


    private IEnumerator ResetDodgeStateCoroutine(float resetTime)
    {
        if (resetTime == 0f) {
            yield return new WaitForSeconds((animator.GetCurrentAnimatorStateInfo(0).length));
        }
        else
        {
            yield return new WaitForSeconds(resetTime); 
        }
        dodgeSkill.ResetSkill();
        Debug.Log("Dodge reset.");
    }

    void Shoot()
    {
        Debug.Log("Shoot() method is being called");
        PlaySoundEffect(shootingSound);

        Vector3 aimPoint = GetAimPoint();
        Vector3 aimDirection = aimPoint - projectileSpawnPoint.transform.position;

        if (Physics.Raycast(projectileSpawnPoint.transform.position, aimDirection.normalized, out RaycastHit raycastHit, aimLayerMask))
        {
            StartCoroutine(SimulateBullet(raycastHit.point, projectileSpawnPoint.transform.position));
        }

        // Instantiate the projectile at the spawn point
        Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, Quaternion.LookRotation(aimDirection, Vector3.up));

        ammoIndicators[currAmmo].canvasRenderer.SetAlpha(0.2f);
        currAmmo -= 1;
    }

    private Vector3 GetAimPoint()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit raycastHit, aimLayerMask))
        {
            Debug.DrawLine(Camera.main.transform.position, raycastHit.point, Color.blue, 2f, false);
            return raycastHit.point; // Adds camera forward such that the aim point stops inside the target 
        }
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * 10000f, Color.red, 2f, false);
        return Camera.main.transform.forward * 10000f;
    }

    private IEnumerator SimulateBullet(Vector3 hitPosition, Vector3 shotPosition)
    {
        yield return new WaitForSeconds(Math.Abs(Vector3.Distance(shotPosition, hitPosition)) / projectileSpeed);
        Debug.DrawLine(shotPosition, hitPosition, Color.green, 2f);
        if (Physics.Raycast(shotPosition, hitPosition - shotPosition, out RaycastHit raycastHit, aimLayerMask))
        {
            Rigidbody targetRB = raycastHit.rigidbody;
            if (raycastHit.rigidbody != null && LayerMask.LayerToName(raycastHit.rigidbody.gameObject.layer) == "Enemy")
            {
                raycastHit.rigidbody.gameObject.GetComponent<EnemyBase>().TakeDamage(projectileDamage, 0);
            }
        }
    }

    public IEnumerator WaitToReload(float duration)
    {
        isReloading = true;
        yield return new WaitForSeconds(duration);
        foreach (KeyValuePair<int, UnityEngine.UI.Image> ammo in ammoIndicators)
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
        PlaySoundEffect(itemPickupSound, 1, 0.5f);
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
        Upgrade fleshWound = upgradeFlags.getUpgradeFlag("Flesh Wound");
        if (fleshWound != null) {
            if (UnityEngine.Random.Range(0f, 1f) <= fleshWound.playerMod.modChance)
            {
                Debug.Log("Flesh wound blocked " + damage * fleshWound.playerMod.modValue + " damage!");
                damage -= damage * fleshWound.playerMod.modValue;
            }
        }

        if (health > 0) {
            base.TakeDamage(damage, additionalDelay);
            animator.Play("GetHit");
        }
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
        ItemProperties properties = GetItemProperties("Health Potion");
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
        ItemProperties properties = GetItemProperties("Health Potion");
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
        if (coinsAreFlushing || isShopping)
        {
            coinsAreFlushing = false;
            coins += coinFlushCounter + addedCoins;
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

    protected virtual void Die()
    {
        animator.Play("Die");
        canMove = false;
    }

    public void PlaySlayHitSound()
    {
        PlaySoundEffect(slayHitSound);
    }
}
