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

    public string upgradeName;
    public string description;
    public UpgradeType upgradeType;
    public int cost;
    public Sprite shopArt;
    public Sprite toolbarArt;

    public Upgrade(string name, string description, UpgradeType upgradeType, int cost, Sprite shopArt, Sprite toolbarArt)
    {
        this.cost = cost;
        this.upgradeName = name;
        this.description = description;
        this.upgradeType = upgradeType;
        this.cost = cost;
        this.shopArt = shopArt;
        this.toolbarArt = toolbarArt;
    }
}
