using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkSkill : ISkill
{
    public List<Upgrade> followingUpgrades;
    public DropTables.Rarity rarity;
    public float cooldown;
    private float lastUsedTime;
    private PlayerController player;
    private SkillType skillType;

    public BlinkSkill(List<Upgrade> followingUpgrades, DropTables.Rarity rarity, float cldwn, SkillType skillType)
    {
        this.followingUpgrades = followingUpgrades;
        this.rarity = rarity;
        this.cooldown = cldwn;
        this.skillType = skillType;
    }

    // Method to use the skill
    public void UseSkill()
    {
        if (CanUseSkill())
        {
            lastUsedTime = Time.time; // Update the last used time
            // Ex. Handle animations and do damage to target here.
            Debug.Log("Blink used.");
        }
        else
        {
            Debug.Log("Blink is on cooldown.");
        }
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

    public DropTables.Rarity GetRarity()
    {
        return rarity;
    }
    public void SetCharacter(PlayerController player)
    {
        this.player = player;
    }

    public SkillType GetSkillType()
    {
        return skillType;
    }
}
