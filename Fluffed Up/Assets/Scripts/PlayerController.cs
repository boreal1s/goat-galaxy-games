using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    // Player attributes
    public float moveSpeed;
    public float rotationSpeed;
    public bool isRunning;
    public UnityEvent<float> AttackEvent;
    public UnityEvent<float> DamageEvent;
    private bool isAttacking = false;
    private float health;
    private float attackPower;

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
        jumpCooldown = 1.5f;
        airSpeedMultiplier = 0.6f;
        attackPower = 25f;
        health = 100f;

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
    }

    private void Jump(float modifier)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(rb.transform.up * jumpForce, ForceMode.Impulse);
        Invoke("ResetJump", jumpCooldown * modifier);
    }

    private void ResetJump()
    {
        isJumping = false;
    }

    void attackEnemy()
    {
        animator.Play("Attack01");
        isAttacking = true;

        // Reset the attacking state after the attack animation finishes
        StartCoroutine(ResetAttackState());

        // TODO Just a temporary solution to hitting enemies. Planning on using events.
        // Find nearby enemies
        AttackEvent?.Invoke(attackPower);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health < 0) health = 0; // Prevent negative health

        // Update the health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }

        if (health <= 0)
        {
            // Character should die here
        }
    }

    IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false; // Reset attacking state after the action is done
    }
}
