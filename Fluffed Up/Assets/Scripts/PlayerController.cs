using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : CharacterClass
{
    // Player attributes
    public UnityEvent<float> AttackEvent;
    public UnityEvent<float> DamageEvent;

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
        healthBar.SetHealth(health);

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
