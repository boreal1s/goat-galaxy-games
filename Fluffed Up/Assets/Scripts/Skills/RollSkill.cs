using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollSkill : Skill, ISkill
{
    CharacterClass player;

    public RollSkill(string name, string desc, List<Upgrade> followingUpgrades, UpgradeType upgradeType, int cost, float dmg, float cldwn, CharacterClass player, float dur = 0f) : base(name, desc, followingUpgrades, upgradeType, cost, dmg, cldwn, dur)
    {
        this.player = player;
    }

    // Method to use the skill
    public void UseSkill()
    {
        if (CanUseSkill())
        {
            lastUsedTime = Time.time; // Update the last used time
            // Ex. Handle animations and do damage to target here.
            Debug.Log($"{name} used.");
        }
        else
        {
            Debug.Log($"{name} is on cooldown.");
        }
    }

    // Check if the skill can be used based on cooldown
    public bool CanUseSkill()
    {
        return (Time.time >= lastUsedTime + cooldown);
    }
}
