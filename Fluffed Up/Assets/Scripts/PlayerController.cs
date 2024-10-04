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

    [SerializeField]
    private Transform cameraTransform;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody>();

        moveSpeed = 5f;
        rotationSpeed = 360f;
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

        #region Camera Relative Movement

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = vertical * camForward;
        Vector3 rightRelataive = horizontal * camRight;

        Vector3 moveDir = forwardRelative + rightRelataive;

        #endregion

        Vector3 move = new Vector3(moveDir.x, 0f, moveDir.z);
        move.Normalize();

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        if (move != Vector3.zero)
        {
            isRunning = true;
            //Quaternion toRotation = Quaternion.LookRotation(forwardRelative, Vector3.up);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            isRunning = false;
        }
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
