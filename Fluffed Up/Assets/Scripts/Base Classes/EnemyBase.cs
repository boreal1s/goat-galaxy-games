using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using System;
using Unity.Mathematics;

public class EnemyBase : CharacterClass
{
    public enum EnemyState
    {
        ChasingPlayer,
        Idle,
        InitiateAttack,
        Attacking,
        Dead,
        Dizzy,
        Disengaging, // Cyclopes Boss only
        Evolving, // Cyclopes Boss only
    };

    [System.Serializable]
    public class ItemDrop
    {
        public GameObject prefab;
        public float dropChance; // Probability as a percentage (0 to 100)
    }

    // Enemy base stats
    public float baseHealth;
    public float baseAttackPower;

    // Enemy Events
    public UnityEvent<float, int> AttackEvent; // input: damage and attack time delay
    public UnityAction OnEnemyDeath; // Trigger to remove event listner in player

    // AI to track player
    public NavMeshAgent navMeshAgent;
    public EnemyState enemyState;
    public PlayerController player; // Player object to be set by WaveManager
    public double actionDelayDefaultInMilli;
    protected double additionalDelayInMilli; // Delay dealt by player's type
    protected float distanceToPlayer;
    protected DateTime lastActionTimestamp;

    // Enemy Drops
    [SerializeField]
    private List<ItemDrop> itemDrops; // List of item drops with chances

    [SerializeField]
    public int goldValueMin;
    public int goldValueMinBase;

    [SerializeField]
    public int goldValueMax;
    public int goldValueMaxBase;

    private PlayerController playerController;
    public GameObject FloatingTextPrefab;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyState = EnemyState.Idle;
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Temporary initialization since this is the base.
        // However, we can utilize this method for inherited classes.
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
            healthBar.SetMaxHealth(health);

        playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null)
            Debug.Log("EnemyBase could not find PlayerController");
    }

    void Update()
    {
        AIStateMachine();
        navMeshAgent.speed = moveSpeed;
    }

    public virtual void AIStateMachine()
    {
        if(player)
        {
            distanceToPlayer = GetDistanceToPlayer();
            switch (enemyState)
            {
                case EnemyState.Idle:
                    // Debug.Log("EnemyState: Idle");
                    if (IsEnemyFarFromPlayer() || IsPlayerOutOfRange())
                        enemyState = EnemyState.ChasingPlayer;
                    break;
                case EnemyState.ChasingPlayer:
                    // Debug.Log("EnemyState: ChasingPlayer");
                    if (IsEnemyFarFromPlayer())
                    {
                        navMeshAgent.isStopped = false;
                        navMeshAgent.SetDestination(player.transform.position);
                    }
                    else if (IsPlayerOutOfRange())
                    {
                        // Rotate to face the player
                        navMeshAgent.isStopped = false;
                        Vector3 direction = (player.transform.position - transform.position).normalized;
                        direction.y = 0; // Keep the rotation on the horizontal plane
                        Quaternion lookRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4);
                    }
                    else //TODO: if player is dead, enemy should go to idle. 
                    {
                        navMeshAgent.isStopped = true;
                        enemyState = EnemyState.InitiateAttack;
                    }
                    break;
                case EnemyState.Dead:
                    navMeshAgent.SetDestination(transform.position); // Stop enemy chasing player
                    break;
                default:
                    break;
            }
        }
    }

    public virtual float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    protected bool IsEnemyFarFromPlayer()
    {
        return distanceToPlayer > attackDistanceThreshold;
    }

    protected virtual bool IsPlayerOutOfRange()
    {
        if (player)
        {
            return math.abs(Vector3.Angle(transform.forward, player.transform.position - transform.position)) > 28 ;
        }
        return false;
    }

    public void InitializeStat(float health, float attackPower)
    {
        this.health = health;
        this.maxHealth = health;
        this.attackPower = attackPower;
    }

    public virtual void Attack()
    {
        AttackEvent?.Invoke(attackPower, attackDelayInMilli);
    }

    public override void TakeDamage(float damage, int additionalDelay)
    {
        navMeshAgent.SetDestination(transform.position);
        base.TakeDamage(damage, additionalDelay);

        if (FloatingTextPrefab) {
            ShowFloatingText(damage);
        }
    }

    void ShowFloatingText(float damage)
    {
        var textMesh = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
        textMesh.GetComponent<TextMesh>().text = damage.ToString();
    }

    protected override void Die()
    {
        enemyState = EnemyState.Dead;
        DisableColliders();
        int coinDrop = UnityEngine.Random.Range(goldValueMin, goldValueMax);
        playerController.UpdateCoins(coinDrop);

        // Randomly select an item to drop based on drop chances
        if (itemDrops.Count > 0)
        {
            float randomValue = UnityEngine.Random.Range(0f, 100f); // Random value between 0 and 100
            float cumulativeChance = 0f;

            foreach (var itemDrop in itemDrops)
            {
                cumulativeChance += itemDrop.dropChance;

                if (randomValue <= cumulativeChance)
                {
                    Vector3 dropPosition = transform.position + new Vector3(0, 0.5f, 0);
                    Instantiate(itemDrop.prefab, dropPosition, Quaternion.identity);
                    Debug.Log("Dropped item: " + itemDrop.prefab.name);
                    break; // Exit loop after dropping an item
                }
            }
        }

        OnEnemyDeath?.Invoke();

        // game object will be destroyed by DieCoroutine
    }

    protected void DisableColliders()
    {
        Destroy(GetComponent<SphereCollider>());
    }

    protected void markLastActionTimeStamp(int additionalMilliToAdd = 0)
    {
        lastActionTimestamp = DateTime.Now;
        lastActionTimestamp = lastActionTimestamp.AddMilliseconds(additionalMilliToAdd);

    }

    protected double getTimePassedLastActionInMilli()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timePassed = currentTime - lastActionTimestamp;
        return timePassed.TotalMilliseconds;
    }

    public bool isAttackInvalid()
    {
        return (enemyState == EnemyState.Dizzy) || (enemyState == EnemyState.Dead);
    }
}
