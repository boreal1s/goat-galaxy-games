using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using System;

public class EnemyBase : CharacterClass
{
    public enum EnemyState
    {
        ChasingPlayer,
        Idle,
        InitiateAttack,
        Attacking,
        Dead
    };

    [System.Serializable]
    public class ItemDrop
    {
        public GameObject prefab;
        public float dropChance; // Probability as a percentage (0 to 100)
    }

    // Enemy Events
    public UnityEvent<float> AttackEvent;
    public UnityEvent<float> DamageEvent;
    public UnityAction OnEnemyDeath; // Trigger to remove event listner in player

    // AI to track player
    public NavMeshAgent navMeshAgent;
    public EnemyState enemyState;
    public PlayerController player; // Player object to be set by WaveManager
    protected float distanceToPlayer;

    // Enemy Drops
    [SerializeField]
    private List<ItemDrop> itemDrops; // List of item drops with chances

    [SerializeField]
    private int goldValueMin;

    [SerializeField]
    private int goldValueMax;

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
    }

    public virtual void AIStateMachine()
    {
        if(player)
        {
            distanceToPlayer  = (transform.position - player.transform.position).magnitude;
            switch (enemyState)
            {
                case EnemyState.Idle:
                    // Debug.Log("EnemyState: Idle");
                    if (distanceToPlayer > attackDistanceThreshold)
                        enemyState = EnemyState.ChasingPlayer;
                    break;
                case EnemyState.ChasingPlayer:
                    // Debug.Log("EnemyState: ChasingPlayer");
                    if (distanceToPlayer > attackDistanceThreshold)
                    {
                        navMeshAgent.SetDestination(player.transform.position);
                    }
                    else //TODO: if player is dead, enemy should go to idle. 
                    {
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

    public void InitializeStat(float health, float attackPower)
    {
        this.health = health;
        this.maxHealth = health;
        this.attackPower = attackPower;
    }

    public virtual void Attack()
    {
        AttackEvent?.Invoke(attackPower);
    }

    public override void TakeDamage(float damage)
    {
        navMeshAgent.SetDestination(transform.position);
        base.TakeDamage(damage);

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
}
