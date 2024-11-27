using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using JetBrains.Annotations;

public class EnemySlimeAndTurtle : EnemyBase
{
    public AudioClip monsterBiteSound;

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
        switch (enemyState)
        {
        case EnemyState.ChasingPlayer:
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
    }

    public override void Attack()
    {
        PlaySoundEffect(monsterBiteSound, hitSoundPitch);
        animator.Play("Attack01");
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
        if (health > 0)
        {
            animator.StopPlayback();
            animator.Play("GetHit");
            enemyState = EnemyState.Dizzy;
        }
    }

    protected override void Die()
    {
        animator.StopPlayback();
        animator.Play("Die");
        StartCoroutine(DieCoroutine(animator.GetCurrentAnimatorStateInfo(0).length*2));

        base.Die();
    }

    public override bool isAttackInvalid()
    {
        if (GetDistanceToPlayer() > attackValidDistanceThreshold)
        {
            return true;
        }
        
        return base.isAttackInvalid();
    }
}
