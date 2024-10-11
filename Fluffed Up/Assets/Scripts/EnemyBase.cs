using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class EnemyBase : CharacterClass
{
    public enum EnemyState
    {
        ChasingPlayer,
        Idle,
    };

    // Enemy Events
    public UnityEvent<float> AttackEvent;
    public UnityEvent<float> DamageEvent;
    public UnityAction OnEnemyDeath; // Trigger to remove event listner in player

    // AI to track player
    public NavMeshAgent navMeshAgent;
    public EnemyState enemyState;
    public PlayerController player; // Player object to be set by WaveManager

    // Enemy Drops
    public List<GameObject> itemDropPrefabs;

    void Start()
    {
        enemyState = EnemyState.Idle;
        navMeshAgent = GetComponent<NavMeshAgent>();
        // Temporary initialization since this is the base.
        // However, we can utilize this method for inherited classes.
        InitializeStat(100f, 1f);
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(health);
        }
    }

    void Update()
    {
        PlayerChaseStateMachine();
    }

    private void PlayerChaseStateMachine()
    {
        float distance = (transform.position - player.transform.position).magnitude;
        switch (enemyState)
        {
        case EnemyState.Idle:
            if (distance > 1)
                enemyState = EnemyState.ChasingPlayer;
            break;
        case EnemyState.ChasingPlayer:
            if (distance > 1)
            {
                navMeshAgent.SetDestination(player.transform.position);
            }
            else
            {
                enemyState = EnemyState.Idle;
            }
            break;
        default:
            break;
        }
    }

    public void InitializeStat(float health, float attackPower)
    {
        this.health = health;
        this.maxHealth = health;
        this.attackPower = attackPower;
    }

    public virtual void Attack(GameObject target)
    {
        if (AttackEvent != null)
        {
            AttackEvent.Invoke(attackPower);
        }
    }

    protected override void Die()
    {
        // Randomly select an item to drop
        if (itemDropPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, itemDropPrefabs.Count);
            Instantiate(itemDropPrefabs[randomIndex], transform.position, Quaternion.identity);
            Debug.Log("Dropped item: " + itemDropPrefabs[randomIndex].name);
        }
        OnEnemyDeath?.Invoke();

        // Destroy game object from parent CharacterClass
        base.Die();
    }
}
