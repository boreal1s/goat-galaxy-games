using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkill : ISkill
{
    public List<Upgrade> followingUpgrades;
    public DropTables.Rarity rarity;
    public float cooldown;
    private float lastUsedTime;
    private PlayerController player;
    private SkillType skillType;
    private int invincibilityFrames;
    private float moveDistancePerFrame;
    private float duration;
    private AudioClip dashSound;

    public DashSkill(List<Upgrade> followingUpgrades, DropTables.Rarity rarity, SkillType skillType, AudioClip ds)
    {
        this.followingUpgrades = followingUpgrades;
        this.dashSound = ds;
        this.rarity = rarity;
        cooldown = 1.8f;
        moveDistancePerFrame = 2.5f;
        duration = 0.1f;
    }

    // Method to use the skill
    public void UseSkill()
    {
        if (CanUseSkill())
        {
            lastUsedTime = Time.time; // Update the last used time
            player.currInvincibilityFrames = invincibilityFrames;
            player.isDodging = true;
            player.ResetDodgeState(duration);
            player.animator.SetBool("dash", true);
            player.PlaySoundEffect(dashSound);

            Debug.Log("Dash used.");
        }
        else
        {
            Debug.Log("Dash is on cooldown.");
        }
    }

    public void ResetSkill()
    {
        player.animator.SetBool("dash", false);
        player.isDodging = false;
    }

    // Check if the skill can be used based on cooldown
    public bool CanUseSkill()
    {
        return (Time.time >= lastUsedTime + cooldown);
    }

    public List<Upgrade> GetFollowingUprages()
    {
        return followingUpgrades;
    }

    public void SetCharacter(ref PlayerController player)
    {
        this.player = player;
    }

    public DropTables.Rarity GetRarity()
    {
        return rarity;
    }
    public SkillType GetSkillType()
    {
        return skillType;
    }

    public float GetSkillValue()
    {
        return moveDistancePerFrame;
    }

}
