using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using JetBrains.Annotations;

public class EnemyTurtle : EnemyBase
{
    private const int ACTION_DELAY_DEFAULT = 800;
    private int actionDelay = ACTION_DELAY_DEFAULT; // give delay in action because slime is stupid!
    public override void AIStateMachine()
    {
        if (actionDelay > 0)
        {
            actionDelay--;
            return;
        }

        base.AIStateMachine();
        switch (enemyState)
        {
        case EnemyState.ChasingPlayer:
            break;
        case EnemyState.InitiateAttack:
            base.AIStateMachine();
            Attack();
            enemyState = EnemyState.Attacking;
            actionDelay = ACTION_DELAY_DEFAULT;
            break;
        case EnemyState.Attacking:
            base.AIStateMachine();
            if (distanceToPlayer > 1)
            {
                actionDelay = ACTION_DELAY_DEFAULT;
                enemyState = EnemyState.ChasingPlayer;
            }
            else if (isAttacking == false)
            {
                actionDelay = ACTION_DELAY_DEFAULT;
                enemyState = EnemyState.InitiateAttack;
            }
            break;
        default:
            base.AIStateMachine();
            break;
        }
    }

    public override void Attack()
    {
        animator.Play("Attack01");
        isAttacking = true;
        // Reset the attacking state after the attack animation finishes
        StartCoroutine(ResetAttackState());

        base.Attack();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        actionDelay = ACTION_DELAY_DEFAULT;
        animator.StopPlayback();
        animator.Play("GetHit");
    }
}
