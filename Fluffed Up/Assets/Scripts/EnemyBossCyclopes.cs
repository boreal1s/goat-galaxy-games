using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using JetBrains.Annotations;
using Unity.Mathematics;

public class EnemyBossCyclopes : EnemyBase
{
    private double idleHeight = 1.5;
    private double attackHeight = -0.5;
    public GameObject rootObject;
    public int fullAttackDurationInMilli;

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
            if (AdjustHeight(true))
            {
                Attack();
                enemyState = EnemyState.Attacking;
                markLastActionTimeStamp(fullAttackDurationInMilli);                
            }
            break;
        case EnemyState.Attacking:
            if (IsEnemyFarFromPlayer() || IsPlayerOutOfRange())
            {
                enemyState = EnemyState.Disengaging;
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

    private bool AdjustHeight(bool engage)
    {
        float currentHeight = navMeshAgent.baseOffset;
        float step = 0.02f;
        double permittedError = 0.05;
        double targetHeight;

        if (engage) targetHeight = attackHeight;
        else targetHeight = idleHeight;

        if (permittedError < targetHeight - currentHeight)
        {
            navMeshAgent.baseOffset = currentHeight + step;
            return false;
        }
        else if (currentHeight - targetHeight > permittedError)
        {
            navMeshAgent.baseOffset = currentHeight - step;
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

    public override float GetDistanceToPlayer()
    {
        // Measure the distance as if the player and the enemy is on the same plane
        Vector3 enemyTransform = transform.position;
        enemyTransform.y = 0;
        Vector3 playerTransform = player.transform.position;
        playerTransform.y = 0;
        float theDistance = Vector3.Distance(enemyTransform, playerTransform);
        Debug.Log("cyclopes distance called, distance is " + theDistance);
        return theDistance;
    }

    protected override bool IsPlayerOutOfRange()
    {
        // Measure the angle as if the player and the enemy is on the same plane
        Vector3 enemyTransform = transform.position;
        enemyTransform.y = 0;
        Vector3 playerTransform = player.transform.position;
        playerTransform.y = 0;
        return math.abs(Vector3.Angle(transform.forward, playerTransform - enemyTransform)) > 28 ;
    }
}
