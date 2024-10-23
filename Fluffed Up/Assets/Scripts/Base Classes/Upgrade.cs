using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public enum UpgradeType
    {
        Skill = 0,
        StatUpgrade = 1,
        PlayerModification = 2,
        GameModification = 3,
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
