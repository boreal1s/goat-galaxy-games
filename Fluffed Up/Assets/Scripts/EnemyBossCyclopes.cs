using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using JetBrains.Annotations;
using Unity.Mathematics;

public class EnemyBossCyclopes : EnemyBase
{
    private double heightDrop = 1.0;
    private double idleHeight;
    private double attackHeight;
    public GameObject rootObject;

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
            AdjustHeight(false);
            isMoving = true;
            break;
        case EnemyState.Disengaging:
            if (AdjustHeight(false))
            {
                enemyState = EnemyState.ChasingPlayer;
            }
            break;
        case EnemyState.InitiateAttack:
            // idleHeight = transform.position.y;
            // attackHeight = idleHeight - heightDrop;
            // enemyState = EnemyState.Engaging;
            // break;
        case EnemyState.Engaging:
            // if (AdjustHeight(true))
            // {
                Attack();
                enemyState = EnemyState.Attacking;
                markLastActionTimeStamp(attackDelayInMilli);                
            // }
            break;
        case EnemyState.Attacking:
            if (IsEnemyFarFromPlayer() || IsPlayerOutOfRange())
            {
                enemyState = EnemyState.ChasingPlayer;//EnemyState.Disengaging;
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

    // private bool AdjustHeight(bool engage)
    // {
    //     Vector3 newPosition = transform.position;
    //     double newAngle;
    //     double step = math.PI/180;
    //     double permittedError = math.PI/180*2;

    //     double targetAngle;
    //     double currentAngle;
    //     double originalYInCos;
    //     if (engage)
    //     {
    //         originalYInCos = newPosition.y - attackHeight - 1;
    //         targetAngle = math.PI;
    //     }
    //     else
    //     {
    //         // Disengage, restore original height
    //         originalYInCos = idleHeight - newPosition.y + 1;
    //         targetAngle = 0.0;
    //     }
    //     currentAngle = math.acos(originalYInCos);

    //     if (permittedError < targetAngle - currentAngle)
    //     {
    //         newAngle = currentAngle + step;
    //         if (newAngle > targetAngle) newAngle = targetAngle;
    //     }
    //     else if (currentAngle - targetAngle > permittedError)
    //     {
    //         newAngle = currentAngle - step;
    //         if (newAngle < targetAngle) newAngle = targetAngle;
    //     }
    //     else return true; // No need to adjust the height anymore, ready to attack.

    //     transform.Translate(0, (float)originalYInCos - math.cos((float)newAngle), 0);

    //     return false;
    // }

    private bool AdjustHeight(bool engage)
    {
        Vector3 currentPosition = transform.position;
        float step = 0.01f;
        double permittedError = 0.05f;
        double targetHeight;

        if (engage) targetHeight = attackHeight;
        else targetHeight = idleHeight;

        if (permittedError < targetHeight - currentPosition.y)
        {
            transform.Translate(0, step, 0);
            return false;
        }
        else if (currentPosition.y - targetHeight > permittedError)
        {
            transform.Translate(0, -step, 0);
            return false;
        }
        else return true; // No need to adjust the height anymore, ready to attack or chase
    }

    public override void Attack()
    {
        animator.Play("Attack03");
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
}
