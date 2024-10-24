using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : Upgrade
{
    public float power;                //  Quantitative effect of the skill
    public float cooldown;              // Cooldown time before the skill can be used again
    public float duration;              // How long the skill lasts (if applicable)
    protected float lastUsedTime;         // Tracks the last time the skill was used

    public Skill(string name, string desc, List<Upgrade> followingUpgrades, UpgradeType upgradeType, int cost, float pwr, float cldwn, float dur = 0f) : base(name, desc, followingUpgrades, upgradeType, cost)
    {
        this.power = pwr;
        this.cooldown = cldwn;
        this.duration = dur;
        this.lastUsedTime = -cldwn; // Ensure the skill can be used immediately
    }
}
