using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CharacterClass : MonoBehaviour
{
    #region Character Attributes
    // Movement
    public float moveSpeed;
    public float rotationSpeed;
    public bool isRunning;

    // Health
    public HealthBar healthBar;
    public float health;
    public float maxHealth;

    // Attack
    public float attackPower;
    public float attackSpeed;
    public float maxAttackSpeed;
    public float attackDistanceThreshold;
    public bool isAttacking = false;
    public int attackDelayInMilli;      // Attack delay in milliseconds. After the delay, the distance between enemy and player is calculated to decide if attack was valid or not.

    // Defense
    public float defense;
    #endregion

    #region Dodging
    public ISkill dodgeSkill;
    public bool isDodging;
    public int currInvincibilityFrames;
    #endregion

    #region Character Status Effects
    // Status effects
    public bool isBurning;
    public bool isFrozen;
    public float burnDamage;
    public float burnDuration;
    public float freezeDuration;
    #endregion

    #region Grounded Attributes
    [Header("Grounded Attributes")]
    public bool isGrounded;
    public LayerMask groundMask;
    #endregion

    #region Jump Attributes
    [Header("Jump Attributes")]
    public float jumpForce;
    public float jumpCooldown;
    public float jumpTime;
    public float airSpeedMultiplier;
    public float gravityMultiplier;
    public bool isJumping;
    #endregion

    #region Shooting Attributes
    protected int maxAmmo = 13;
    protected int currAmmo;
    protected float reloadTime = 2f;
    protected float shotTime = 0.4f;
    public bool isReloading;
    protected Dictionary<int, Image> ammoIndicators;
    #endregion

    // Character animator and rigidbody
    public Animator animator;
    public Rigidbody rb;
    public Sound3D sound3DPrefab;

    // Sound Effect Audio Clip
    #region Sound Effects for Character Class
    [Header("Sound Effects for Character Class")]
    public AudioClip hitSoundEffect;
    public float hitSoundPitch;
    public AudioClip healSoundEffect;    
    private bool loopSoundIsPlaying = false;
    private Sound3D loopSound3D;
    #endregion

    public void Jump(float modifier)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(rb.transform.up * jumpForce, ForceMode.Impulse);
        Invoke("ResetJump", jumpCooldown * modifier);
    }

    public void ResetJump()
    {
        isJumping = false;
    }

    public IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds((animator.GetCurrentAnimatorStateInfo(0).length / attackSpeed));
        isAttacking = false; // Reset attacking state after the action is done
    }

    public void ApplyBurn(float duration)
    {
        if (!isBurning)
        {
            isBurning = true;
            StartCoroutine(BurnCoroutine(duration));
        }
    }

    private IEnumerator BurnCoroutine(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            TakeDamage(burnDamage * Time.deltaTime, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isBurning = false; // Reset burning state
    }

    public void ApplyFreeze(float duration)
    {
        if (!isFrozen)
        {
            Debug.Log($"{gameObject.name} is frozen.");
            isFrozen = true;
            float origMoveSpeed = moveSpeed;
            moveSpeed = 0; // Stop movement
            StartCoroutine(FreezeCoroutine(origMoveSpeed, duration));
        }
    }

    private IEnumerator FreezeCoroutine(float origMoveSpeed, float duration)
    {
        yield return new WaitForSeconds(duration);
        moveSpeed = origMoveSpeed; // Reset to original speed
        isFrozen = false; // Reset frozen state
    }

    public IEnumerator DieCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    public virtual void updateMaxHealth(float maxHealthChange)
    {
        maxHealth += maxHealthChange;
        healthBar.SetMaxHealth(maxHealth);
        if (maxHealthChange > 0)
            Heal(maxHealthChange);
    }

    public virtual void Heal(float amount)
    {
        PlaySoundEffect(healSoundEffect);
        health = Mathf.Clamp(health + amount, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }
    }

    public virtual void TakeDamage(float damage, int additionalDelay)
    {
        if (currInvincibilityFrames < 1)
        {
            PlaySoundEffect(hitSoundEffect, hitSoundPitch);

            float mitigatedDamage = Mathf.Clamp(damage - defense, 0, damage);
            //Debug.Log($"{gameObject.name} is taking {damage} damage.");
            health = health - mitigatedDamage;
            //Debug.Log($"{gameObject.name} health is now {health}");

            if (health <= 0)
            {
                Die();
            }

            if (healthBar != null)
            {
                healthBar.SetHealth(health);
            }
        }
    }

    public void PlaySoundEffect(AudioClip audioClip, float pitch = 1.0f, float audioLevel = 1.0f)
    {
        if (sound3DPrefab)
        {
            // Debug.Log("CharacterClass PlaySoundEffect!!!!");
            Sound3D sound3DObject = Instantiate(sound3DPrefab, transform.position, Quaternion.identity, null);
            sound3DObject.audioSrc.clip = audioClip;

            sound3DObject.audioSrc.minDistance = 5f;
            sound3DObject.audioSrc.maxDistance = 100f;
            sound3DObject.audioSrc.pitch = pitch;
            sound3DObject.audioSrc.volume = audioLevel;

            sound3DObject.audioSrc.Play();
        }
    }

    public void PlaySoundEffectInALoop(AudioClip audioClip, float pitch = 1.0f)
    {
        if (sound3DPrefab)
        {
            if (loopSoundIsPlaying == false)
            {
                loopSoundIsPlaying = true;
                loopSound3D = Instantiate(sound3DPrefab, transform.position, Quaternion.identity, null);
                loopSound3D.audioSrc.clip = audioClip;

                loopSound3D.audioSrc.minDistance = 5f;
                loopSound3D.audioSrc.maxDistance = 100f;
                loopSound3D.audioSrc.loop = true;
                loopSound3D.audioSrc.pitch = pitch;

                loopSound3D.audioSrc.Play();
            }
        }
    }

    public void StopPlaySoundEffectInALoop()
    {
        if (loopSoundIsPlaying == true)
        {
            loopSound3D.audioSrc.Stop();
            loopSoundIsPlaying = false;
        }            
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        if(gameObject.name == "PlayerSlayer(Clone)" || gameObject.name == "PlayerShooter(Clone)") {
            StartCoroutine(DelayedDeathActions()); // Start the coroutine to delay actions
        }
        else {
            Destroy(gameObject);
        }
    }

    private IEnumerator DelayedDeathActions()
    {
        // Wait for a few seconds before showing death screen. Death animation needs to finish
        yield return new WaitForSeconds(2f);

        // After the delay, unlock the cursor and show it
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Uncomment to load the death scene (if necessary)
        SceneManager.LoadScene("DeathScene");
    }
    
    public void UpdateAttackSpeed(float attackSpeedMultiplier)
    {
        attackSpeed = attackSpeed + (attackSpeed * attackSpeedMultiplier);
        animator.SetFloat("attackSpeed", attackSpeed);

        reloadTime = Mathf.Max(0.1f, reloadTime - (reloadTime * attackSpeedMultiplier));
        shotTime = Mathf.Max(0.01f, shotTime - (shotTime * attackSpeedMultiplier));
    }
}
