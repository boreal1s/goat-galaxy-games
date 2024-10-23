using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : Upgrade
{
    public float damage;                // Damage dealt by the skill
    public float cooldown;              // Cooldown time before the skill can be used again
    public float duration;              // How long the skill lasts (if applicable)
    protected float lastUsedTime;         // Tracks the last time the skill was used

    public Skill(string name, string desc, ArrayList followingUpgrades, UpgradeType upgradeType, int cost, float rarity, float dmg, float cldwn, float dur = 0f) : base(name, desc, followingUpgrades, upgradeType, cost, rarity)
    {
        this.damage = dmg;
        this.cooldown = cldwn;
        this.duration = dur;
        this.lastUsedTime = -cldwn; // Ensure the skill can be used immediately
    }
}
