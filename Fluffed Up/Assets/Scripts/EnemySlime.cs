using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using JetBrains.Annotations;

public class EnemySlime : EnemyBase
{
    private const double ACTION_DELAY_DEFAULT_MS = 500.0;

    public override void AIStateMachine()
    {
        if (getTimePassedLastActionInMilli() < ACTION_DELAY_DEFAULT_MS)
        {
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
            markLastActionTimeStamp();
            break;
        case EnemyState.Attacking:
            base.AIStateMachine();
            if (distanceToPlayer > 2)
            {
                markLastActionTimeStamp();
                enemyState = EnemyState.ChasingPlayer;
            }
            else if (isAttacking == false)
            {
                markLastActionTimeStamp();
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
        markLastActionTimeStamp();
        if (health > 0)
        {
            animator.StopPlayback();
            animator.Play("GetHit");
        }
    }

    protected override void Die()
    {
        animator.StopPlayback();
        animator.Play("Die");
        StartCoroutine(DieCoroutine(animator.GetCurrentAnimatorStateInfo(0).length*2));

        base.Die();
    }
}
