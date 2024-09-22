using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;
    public bool isRunning;
    public GameObject fakeEnemy;
    public float enemyAttachDistanceThreshold = 3.0f;

    private Animator animator;
    private Rigidbody rb;
    private bool isAttacking = false;

    [SerializeField]
    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        moveSpeed = 5f;
        rotationSpeed = 360f;

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

        // Enemy attack control
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            attackEnemy();
        }
    }

    void attackEnemy()
    {
        animator.Play("Attack01");
        isAttacking = true;
        if (fakeEnemy != null)
        {
            float enemyDistance = Vector3.Distance(transform.position, fakeEnemy.transform.position);
            if (enemyDistance <= enemyAttachDistanceThreshold)
            {
                StartCoroutine(DestroyEnemyAfterAttack(true));
            }
            else
            {
                StartCoroutine(DestroyEnemyAfterAttack(false));
            }
        }
        else
        {
            StartCoroutine(DestroyEnemyAfterAttack(false));
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        } else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    IEnumerator DestroyEnemyAfterAttack(bool destroyEnemy)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        
        if (fakeEnemy != null && destroyEnemy == true)
        {
            Destroy(fakeEnemy);
        }

        isAttacking = false; // Reset attacking state after the action is done
    }
}
