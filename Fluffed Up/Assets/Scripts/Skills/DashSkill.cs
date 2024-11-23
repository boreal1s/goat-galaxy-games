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

    public DashSkill(List<Upgrade> followingUpgrades, DropTables.Rarity rarity, float cldwn, SkillType skillType)
    {
        this.followingUpgrades = followingUpgrades;
        this.rarity = rarity;
        this.cooldown = cldwn;
    }

    // Method to use the skill
    public bool UseSkill()
    {
        if (CanUseSkill())
        {
            lastUsedTime = Time.time; // Update the last used time
            // Ex. Handle animations and do damage to target here.
            Debug.Log("Roll used.");
            return true;
        }
        else
        {
            Debug.Log("Roll is on cooldown.");
            return false;
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
}
