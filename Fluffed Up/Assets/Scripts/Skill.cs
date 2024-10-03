using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public string skillName;            // Skill Name
    public string description;          // Description of the skill
    public float damage;                // Damage dealt by the skill
    public float cooldown;              // Cooldown time before the skill can be used again
    public float duration;              // How long the skill lasts (if applicable)
    private float lastUsedTime;         // Tracks the last time the skill was used

    public Skill(string name, string desc, float dmg, float cldwn, float dur = 0f)
    {
        skillName = name;
        description = desc;
        damage = dmg;
        cooldown = cldwn;
        duration = dur;
        lastUsedTime = -cldwn; // Ensure the skill can be used immediately
    }

    // Method to use the skill
    public bool UseSkill()
    {
        if (CanUseSkill())
        {
            lastUsedTime = Time.time; // Update the last used time
            // Ex. Handle animations and do damage to target here.
            return true;
        }
        else
        {
            Debug.Log($"{skillName} is on cooldown.");
            return false;
        }
    }

    // Check if the skill can be used based on cooldown
    public bool CanUseSkill()
    {
        return (Time.time >= lastUsedTime + cooldown);
    }
}
