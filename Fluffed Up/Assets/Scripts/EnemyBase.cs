using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBase : MonoBehaviour
{
    public float health;
    public float attackPower;
    public UnityEvent<float> AttackEvent;

    public void InitializeStat(float health, float attackPower)
    {
        this.health = health;
        this.attackPower = attackPower;
    }

    public virtual void OnAttacked(float damage)
    {
        health -= damage;
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
        Destroy(gameObject);
    }

}
