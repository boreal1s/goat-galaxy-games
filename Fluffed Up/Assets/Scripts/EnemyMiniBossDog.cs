using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using JetBrains.Annotations;

public class EnemyMiniBossDog : EnemyBase
{
    private const int ACTION_DELAY_DEFAULT = 500;
    private const int DIZZY_DELAY_DEFAULT = 800;
    private int actionDelay = ACTION_DELAY_DEFAULT; // give delay in action because minibossdog is stupid!
    private int dizzyDelay = 0;

    public override void AIStateMachine()
    {
        if (actionDelay > 0)
        {
            actionDelay--;
            return;
        }

        base.AIStateMachine();

        bool isMoving = false;

        switch (enemyState)
        {
        case EnemyState.ChasingPlayer:
            isMoving = true;
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
        case EnemyState.Dizzy:
            if (--dizzyDelay <= 0)
            {
                enemyState = EnemyState.Attacking;
            }
            break;
        default:
            base.AIStateMachine();
            break;
        }

        animator.SetBool("isMoving", isMoving);
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
        if (health > 0)
        {
            animator.StopPlayback();
            animator.Play("GetHit");
            enemyState = EnemyState.Dizzy;
            dizzyDelay = DIZZY_DELAY_DEFAULT;
        }
    }

    protected override void Die()
    {
        animator.StopPlayback();
        animator.Play("Die01");
        StartCoroutine(DieCoroutine(animator.GetCurrentAnimatorStateInfo(0).length*2));

        base.Die();
    }
}
