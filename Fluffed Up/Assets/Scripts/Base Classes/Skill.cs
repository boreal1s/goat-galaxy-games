using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : Upgrade
{
    public float power;                //  Quantitative effect of the skill
    public float cooldown;              // Cooldown time before the skill can be used again
    public float duration;              // How long the skill lasts (if applicable)
    protected float lastUsedTime;         // Tracks the last time the skill was used
    public DropTables.Rarity rarity;
    public List<Skill> followingUpgrades;

    public Skill(string name, string desc, List<Skill> followingUpgrades, UpgradeType upgradeType, int cost, float pwr, float cldwn, DropTables.Rarity rarity, Sprite shopArt, Sprite toolbarArt, float dur = 0f) : base(name, desc, upgradeType, cost, shopArt, toolbarArt)
    {
        this.power = pwr;
        this.cooldown = cldwn;
        this.duration = dur;
        this.lastUsedTime = -cldwn; // Ensure the skill can be used immediately
        this.rarity = rarity;
        this.followingUpgrades = followingUpgrades;
    }
}
