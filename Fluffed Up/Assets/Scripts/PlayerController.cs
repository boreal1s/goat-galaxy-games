using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : CharacterClass
{
<<<<<<< Updated upstream
    // Player attributes
    public UnityEvent<float> AttackEvent;
    public UnityEvent<float> DamageEvent;
=======
    [Header("Shooting")]
    public GameObject projectilePrefab; // The projectile prefab to instantiate
    public Transform projectileSpawnPoint; // Where the projectile will spawn
    public float projectileSpeed = 20f; // Speed of the projectile
    public float projectileDamage = 10f; // Damage dealt by the projectile
                                         // Player attributes
    public float moveSpeed;
    public float rotationSpeed;
    public bool isRunning;
    private bool isAttacking = false;

    // Interaction with enemy
    public float enemyAttackDistanceThreshold = 1.5f;

    // UI Health HUD
    private HealthBar healthBar;

    // Character animator and rigidbody
    private Animator animator;
    private Rigidbody rb;

    #region Grounded Attributes
    [Header("Grounded Attributes")]
    public bool isGrounded;
    public LayerMask groundMask;
    #endregion

    #region Jump Attributes
    [Header("Jump Attributes")]
    public float jumpForce;
    public float jumpCooldown;
    public float jumpTime;
    public float airSpeedMultiplier;
    public float gravityMultiplier;
    bool isJumping;
    #endregion
>>>>>>> Stashed changes

    [Header("Inputs")]
    [SerializeField]
    private InputMap inputs;

    [Header("Camera")]
    [SerializeField]
    private Transform cameraTransform;

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
        jumpForce = 100f;
        jumpTime = 3f;
        jumpCooldown = 0.5f;
        airSpeedMultiplier = 0.6f;
        attackPower = 25f;
        health = 100f;
        attackDistanceThreshold = 3f;

        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(health);
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        #region Grounded Check
        isGrounded = Physics.CheckSphere(rb.transform.position, 0.2f, groundMask);
        #endregion

        #region Movement Control

        #region Configure Camera Relative Movement

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = vertical * camForward;
        Vector3 rightRelataive = horizontal * camRight;

        Vector3 moveDir = forwardRelative + rightRelataive;

        #endregion

        Vector3 move = new Vector3(moveDir.x, 0f, moveDir.z);

        if (isGrounded)
            rb.MovePosition(transform.position + move * moveSpeed * Time.deltaTime);
        else
            rb.MovePosition(transform.position + move * moveSpeed * airSpeedMultiplier * Time.deltaTime);

        isRunning = move != Vector3.zero && isGrounded;
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isRunning", isRunning);
        Debug.Log("isAttacking:" + isAttacking.ToString());

        // Enemy attack control. Attack when clicking left click.
        // TODO might need to update to input string name to account for controller.
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            attackEnemy();
        }

        if (inputs.jump && isGrounded && !isJumping)
        {
            isJumping = true;
            Jump(1f);
        }
<<<<<<< Updated upstream
        healthBar.SetHealth(health);
=======
        if (Input.GetMouseButtonDown(1) && !isAttacking)
        {
            Shoot();
        }
    }
    void Shoot()
    {
        Debug.Log("Shoot() method is being called");

        // Trigger the shooting animation
        animator.SetTrigger("Shoot");

        // Instantiate the projectile at the spawn point
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        // Ignore collision between the projectile and the player
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());

        // Set the projectile's speed and damage
        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.speed = projectileSpeed;
            projScript.damage = projectileDamage;
        }
        else
        {
            Debug.LogError("Projectile script not found on the projectile prefab.");
        }

        // Set the projectile's direction
        projectile.transform.forward = transform.forward;
    }

>>>>>>> Stashed changes

        if (health <= 0)
        {
            showDeathScreen(true);
        }
    }

    void attackEnemy()
    {
        animator.Play("Attack01");
        isAttacking = true;
        // Reset the attacking state after the attack animation finishes
        StartCoroutine(ResetAttackState());

        // Find nearby enemies
        AttackEvent?.Invoke(attackPower);
    }

    public void showDeathScreen(bool show)
    {
        if (show)
        {
            // show death screen here
        }
    }
}
