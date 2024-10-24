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
        Consumable = 4,
    }

    public string upgradeName;
    public string description;
    public List<Upgrade> followingUpgrades;
    public UpgradeType upgradeType;
    public int cost;

    public Upgrade(string name, string description, List<Upgrade> followingUpgrades, UpgradeType upgradeType, int cost)
    {
        this.cost = cost;
        this.upgradeName = name;
        this.description = description;
        this.followingUpgrades = followingUpgrades;
        this.upgradeType = upgradeType;
        this.cost = cost;
    }
}
