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
    private CharacterClass character;

    public RollSkill(List<Upgrade> followingUpgrades, DropTables.Rarity rarity, float cldwn)
    {
        this.followingUpgrades = followingUpgrades;
        this.rarity = rarity;
        this.cooldown = cldwn;
        this.character = character;
    }

    // Method to use the skill
    public void UseSkill()
    {
        if (CanUseSkill())
        {
            lastUsedTime = Time.time; // Update the last used time
            character.isGrounded = false;
            character.animator.SetTrigger("dodge");
            ResetRollState();
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
        return (Time.time >= lastUsedTime + cooldown) && character.isGrounded;
    }

    public List<Upgrade> GetFollowingUprages()
    {
        return followingUpgrades;
    }

    public DropTables.Rarity GetRarity()
    {
        return rarity;
    }

    public void SetCharacter(CharacterClass character)
    {
        this.character = character;
    }

    private IEnumerator ResetRollState()
    {
        yield return new WaitForSeconds((character.animator.GetCurrentAnimatorStateInfo(0).length));
        character.isGrounded = false;
    }
}
