using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class RollSkill : ISkill
{
    public List<Upgrade> followingUpgrades;
    public DropTables.Rarity rarity;
    public float cooldown;
    private float lastUsedTime;
    private PlayerController player;
    private SkillType skillType;
    private int invincibilityFrames;
    public Vector3 dodgeDir;
    public float dodgeSpeed = 1.3f;

    public RollSkill(List<Upgrade> followingUpgrades, DropTables.Rarity rarity, float cldwn, int iframes, SkillType skillType)
    {
        this.followingUpgrades = followingUpgrades;
        this.rarity = rarity;
        this.cooldown = cldwn;
        lastUsedTime = -1;
        this.invincibilityFrames = iframes;
    }

    // Method to use the skill
    public void UseSkill()
    {
        if (CanUseSkill())
        {
            lastUsedTime = Time.time; // Update the last used time
            player.currInvincibilityFrames = invincibilityFrames;
            player.isDodging = true;
            player.ResetDodgeState();
            player.animator.SetTrigger("roll");

            Debug.Log("Roll used.");
        }
        else
        {
            Debug.Log("Roll is on cooldown.");
        }
    }

    public void ResetSkill()
    {
        player.isDodging = false;
    }

    // Check if the skill can be used based on cooldown
    public bool CanUseSkill()
    {
        return (Time.time >= lastUsedTime + cooldown) && player.isGrounded;
    }

    public List<Upgrade> GetFollowingUprages()
    {
        return followingUpgrades;
    }

    public DropTables.Rarity GetRarity()
    {
        return rarity;
    }

    public void SetCharacter(ref PlayerController player)
    {
        this.player = player;
    }

    public SkillType GetSkillType()
    {
        return skillType;
    }
    public float GetSkillValue()
    {
        return dodgeSpeed;
    }

}
