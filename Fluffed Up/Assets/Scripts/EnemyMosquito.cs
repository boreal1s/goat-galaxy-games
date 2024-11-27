using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using JetBrains.Annotations;

public class EnemyMosquito : EnemyBase
{

    public override void AIStateMachine()
    {
        if (getTimePassedLastActionInMilli() < actionDelayDefaultInMilli + additionalDelayInMilli)
        {
            return;
        }
        else
        {
            additionalDelayInMilli = 0.0f;
        }

        base.AIStateMachine();

        bool isMoving = false;

        switch (enemyState)
        {
        case EnemyState.ChasingPlayer:
            isMoving = true;
            break;
        case EnemyState.InitiateAttack:
            Attack();
            enemyState = EnemyState.Attacking;
            markLastActionTimeStamp(attackDelayInMilli);
            break;
        case EnemyState.Attacking:
            if (IsEnemyFarFromPlayer() || IsPlayerOutOfRange())
            {
                enemyState = EnemyState.ChasingPlayer;
            }
            else if (isAttacking == false)
            {
                enemyState = EnemyState.InitiateAttack;
            }
            break;
        case EnemyState.Dizzy:
            enemyState = EnemyState.Attacking;
            break;
        default:
            break;
        }

        animator.SetBool("isMoving", isMoving);
    }

    public override void Attack()
    {
        // PlaySoundEffect(punchSound, 0.7f);
        animator.Play("Swarm09_Attack");
        isAttacking = true;
        // Reset the attacking state after the attack animation finishes
        StartCoroutine(ResetAttackState());

        base.Attack();
    }

    public override void TakeDamage(float damage, int additionalDelay)
    {
        additionalDelayInMilli = (double)additionalDelay;
        base.TakeDamage(damage, additionalDelay);
        markLastActionTimeStamp();
        animator.SetBool("isMoving", false);
        if (health > 0)
        {
            animator.StopPlayback();
            animator.Play("Swarm09_GetHit");
            enemyState = EnemyState.Dizzy;
        }
    }

    protected override void Die()
    {
        Debug.Log("Die?");
        animator.StopPlayback();
        animator.Play("Swarm09_Die");
        StartCoroutine(DieCoroutine(animator.GetCurrentAnimatorStateInfo(0).length*2));

        base.Die();
    }
}

