using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RollSkill : ISkill
{
    public List<Upgrade> followingUpgrades;
    public DropTables.Rarity rarity;
    public float cooldown;
    private float lastUsedTime;
    private CharacterClass player;
    private SkillType skillType;

    public RollSkill(List<Upgrade> followingUpgrades, DropTables.Rarity rarity, float cldwn, SkillType skillType)
    {
        this.followingUpgrades = followingUpgrades;
        this.rarity = rarity;
        this.cooldown = cldwn;
        lastUsedTime = -1;
    }

    // Method to use the skill
    public void UseSkill()
    {
        if (CanUseSkill())
        {
            lastUsedTime = Time.time; // Update the last used time
            player.isGrounded = false;
            player.animator.SetTrigger("dodge");
            ResetDodgeState();
            Debug.Log("Roll used.");
        }
        else
        {
            Debug.Log("Roll is on cooldown.");
        }
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

    public void SetCharacter(PlayerController player)
    {
        this.player = player;
    }

    public SkillType GetSkillType()
    {
        return skillType;
    }

    private IEnumerator ResetDodgeState()
    {
        yield return new WaitForSeconds((player.animator.GetCurrentAnimatorStateInfo(0).length));
        player.isGrounded = false;
    }
}
