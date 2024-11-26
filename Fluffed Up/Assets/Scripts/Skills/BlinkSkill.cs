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
    private int invincibilityFrames;
    private float moveDistancePerFrame;
    private float tpTime;
    AudioClip tpSound;

    public BlinkSkill(List<Upgrade> followingUpgrades, AudioClip tpSound)
    {
        this.followingUpgrades = followingUpgrades;
        this.tpSound = tpSound;

        rarity = DropTables.Rarity.Rare;
        cooldown = 6f;
        skillType = SkillType.Dodge;
        invincibilityFrames = 5;
        moveDistancePerFrame = 6.5f;
        tpTime = 0.4f;
    }

    // Method to use the skill
    public void UseSkill()
    {
        if (CanUseSkill())
        {
            Debug.Log("Blink used.");
            lastUsedTime = Time.time; // Update the last used time
            player.currInvincibilityFrames = invincibilityFrames;
            player.isDodging = true;

            foreach (SkinnedMeshRenderer sr in player.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                sr.enabled = false;
            }
            foreach (MeshRenderer r in player.gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                r.enabled = false;
            }
            player.PlaySoundEffect(tpSound);
            player.ResetDodgeState(tpTime);
            Debug.Log("Blinked!");
        }
        else
        {
            Debug.Log("Blink is on cooldown.");
        }
    }

    public void ResetSkill()
    {
        player.isDodging = false;
        foreach (SkinnedMeshRenderer sr in player.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            sr.enabled = true;
        }
        foreach (MeshRenderer r in player.gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            r.enabled = true;
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
        return moveDistancePerFrame;
    }
}
