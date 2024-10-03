using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    [SerializeField]
    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody>();

        moveSpeed = 5f;
        rotationSpeed = 360f;

        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(100f);
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

        // TODO Just a temporary solution to hitting enemies. Planning on using events.
        // Find nearby enemies
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyAttackDistanceThreshold);
        foreach (var hitCollider in hitColliders)
        {
            EnemyBase enemy = hitCollider.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                float damage = 10f; // Specify the damage amount
                enemy.TakeDamage(damage); // enemy damaged
            }
        }
        // Reset the attacking state after the attack animation finishes
        StartCoroutine(ResetAttackState());
    }

    IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false; // Reset attacking state after the action is done
    }
}
