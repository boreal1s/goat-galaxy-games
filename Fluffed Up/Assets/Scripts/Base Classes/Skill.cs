using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public float power;                //  Quantitative effect of the skill
    public float cooldown;              // Cooldown time before the skill can be used again
    public float duration;              // How long the skill lasts (if applicable)
    protected float lastUsedTime;         // Tracks the last time the skill was used
    public DropTables.Rarity rarity;
    public List<Upgrade> followingUpgrades;

    public Skill(List<Upgrade> followingUpgrades, float pwr, float cldwn, DropTables.Rarity rarity, float dur = 0f) 
    {
        this.power = pwr;
        this.cooldown = cldwn;
        this.duration = dur;
        this.lastUsedTime = -cldwn; // Ensure the skill can be used immediately
        this.rarity = rarity;
        this.followingUpgrades = followingUpgrades;
    }
}
