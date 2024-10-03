using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBase : MonoBehaviour
{
    // Enemy Attributes
    public float health;
    public float attackPower;

    // Enemy Events
    public UnityEvent<float> AttackEvent;

    // Enemy Health UI
    private HealthBar healthBar;

    // Enemy Drops
    public GameObject itemDropPrefab;
    public List<GameObject> itemDropPrefabs;

    void Start()
    {
        // Temporary initialization since this is the base.
        // However, we can utilize this method for inherited classes.
        InitializeStat(100f, 1f);
    }

    public void InitializeStat(float health, float attackPower)
    {
        this.health = health;
        this.attackPower = attackPower;
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(health);
        }
    }

    public virtual void TakeDamage(float damage)
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
            Die();
        }
    }

    public virtual void Attack(GameObject target)
    {
        if (AttackEvent != null)
        {
            AttackEvent.Invoke(attackPower);
        }
    }

    public virtual void Die()
    {
        // Drop specific item before destroying the enemy
        if (itemDropPrefab != null)
        {
            Instantiate(itemDropPrefab, transform.position, itemDropPrefab.transform.rotation);
        }

        // TODO uncomment for randomly drop prefabs
        // Randomly select an item to drop
        //if (itemDropPrefabs.Count > 0)
        //{
        //    int randomIndex = Random.Range(0, itemDropPrefabs.Count);
        //    Instantiate(itemDropPrefabs[randomIndex], transform.position, Quaternion.identity);
        //    Debug.Log("Dropped item: " + itemDropPrefabs[randomIndex].name);
        //}

        Destroy(gameObject);
    }

}
