using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade
{
    public enum UpgradeType
    {
        Skill = 0,
        SkillUpgrade = 1,
        StatUpgrade = 2,
        PlayerModification = 3,
        GameModification = 4,
    }

    public string name;
    public string description;
    public ArrayList followingUpgrades;
    public UpgradeType upgradeType;
    public int cost;
    public float rarity; // lower value is more rare

    public Upgrade(string name, string description, ArrayList followingUpgrades, UpgradeType upgradeType, int cost, float rarity)
    {
        this.cost = cost;
        this.name = name;
        this.description = description;
        this.followingUpgrades = followingUpgrades;
        this.upgradeType = upgradeType;
        this.cost = cost;
        this.rarity = rarity;
    }
}
