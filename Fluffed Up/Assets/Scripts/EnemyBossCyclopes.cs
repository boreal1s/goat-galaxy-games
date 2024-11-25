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
    private double upAboveHeight = 2.5;
    public GameObject rootObject;
    public int fullAttackDurationInMilli;
    public int attackType2DelayInMilli;
    public int attackType2FullDurationInMilli;
    public UnityEvent<float, int> AttackEventType2; // input: damage and attack time delay
    public AudioClip electrocuteSoundEffect;
    public AudioClip biteSoundEffect;
    public AudioClip angrySoundEffect;

    private bool attackType2Activated = false;
    private int heightAdjustSpeed = 1;

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
            if (attackType2Activated)
            {
                animator.SetBool("isAngry", true);
            }
            else
            {
                animator.SetBool("isAngry", false);
            }
            break;
        case EnemyState.Disengaging:
            if (AdjustHeight())
            {
                enemyState = EnemyState.ChasingPlayer;
            }
            break;
        case EnemyState.InitiateAttack:
            if (attackType2Activated == false)
            {
                if (AdjustHeight())
                {
                    Attack();
                    enemyState = EnemyState.Attacking;
                    markLastActionTimeStamp(fullAttackDurationInMilli);                
                }
            }
            else // Second type attack
            {
                if (AdjustHeight())
                {
                    AttackType2();
                    enemyState = EnemyState.Attacking;
                    markLastActionTimeStamp(attackType2FullDurationInMilli);                
                }
            }
            break;
        case EnemyState.Attacking:
            if (IsEnemyFarFromPlayer() || IsPlayerOutOfRange())
            {
                if (attackType2Activated) StopPlaySoundEffectInALoop();
                enemyState = EnemyState.Disengaging;
            }
            else if (isAttacking == false)
            {
                enemyState = EnemyState.InitiateAttack;
            }
            break;
        case EnemyState.Evolving:
            AdjustHeight();
            // state will be set to Attacking by async timer started by TakeDamage
            break;
        case EnemyState.Dizzy:
            enemyState = EnemyState.Attacking;
            break;
        default:
            break;
        }

        animator.SetBool("isMoving", isMoving);
    }

    private bool AdjustHeight()
    {
        float currentHeight = navMeshAgent.baseOffset;
        float step = 0.02f * heightAdjustSpeed;
        double permittedError = 0.05;
        double targetHeight = 0;

        if (enemyState == EnemyState.InitiateAttack) targetHeight = attackHeight;
        else if (enemyState == EnemyState.Disengaging) targetHeight = idleHeight;
        else if (enemyState == EnemyState.Evolving) targetHeight = upAboveHeight;

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
        PlaySoundEffect(biteSoundEffect, 0.7f);
        animator.Play("Attack03");
        isAttacking = true;
        // Reset the attacking state after the attack animation finishes
        StartCoroutine(ResetAttackState());

        base.Attack();
    }

    private void AttackType2()
    {
        animator.Play("Attack02RPT");
        isAttacking = true;
        // Reset the attacking state after the attack animation finishes
        StartCoroutine(ResetAttackType2State(attackType2FullDurationInMilli));
        AttackEvent?.Invoke(math.ceil(attackPower/30), attackType2DelayInMilli); // This is continous type attack
        PlaySoundEffectInALoop(electrocuteSoundEffect);
    }

    IEnumerator ResetAttackType2State(int delayInMilli)
    {
        yield return new WaitForSeconds(delayInMilli/1000f);
        isAttacking = false; // Reset attacking state after the action is done
    }

    public override void TakeDamage(float damage, int additionalDelay)
    {
        additionalDelayInMilli = (double)additionalDelay;
        base.TakeDamage(damage, additionalDelay);
        animator.SetBool("isMoving", false);
        if (health > maxHealth/2)
        {
            markLastActionTimeStamp(200);
            animator.StopPlayback();
            animator.Play("GetHit");
            enemyState = EnemyState.Dizzy;
        }else if (health > 0 && attackType2Activated == false)
        {
            attackType2Activated = true;
            heightAdjustSpeed = 2;
            navMeshAgent.speed *= heightAdjustSpeed;
            StartCoroutine(WaitForEvolveAnimation(1700)); // Wait for GetHit and Daunting animation
            PlaySoundEffect(angrySoundEffect);
            animator.StopPlayback();
            animator.Play("GetHit");
            enemyState = EnemyState.Evolving;
        }
        else
        {
            markLastActionTimeStamp(100);
            animator.StopPlayback();
            animator.Play("GetHit");
            enemyState = EnemyState.Dizzy;
        }
    }

    IEnumerator WaitForEvolveAnimation(int delayInMilli)
    {
        yield return new WaitForSeconds(delayInMilli/1000f);
        enemyState = EnemyState.Attacking;
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
