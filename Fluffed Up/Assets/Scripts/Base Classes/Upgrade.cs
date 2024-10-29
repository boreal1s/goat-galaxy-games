using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    Skill = 0,
    StatUpgrade = 1,
    PlayerModification = 2,
    GameModification = 3,
    Consumable = 4,
}

public class Upgrade
{
    public string upgradeName;
    public string description;
    public UpgradeType upgradeType;
    public int cost;
    public Sprite shopArt;
    public Sprite toolbarArt;

    public StatUpgrade statUpgrade;
    public ISkill skill;
    public PlayerModification playerMod;
    public GameModification gameMod;
    public Consumable consumable;


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
    public Upgrade(string name, string description, UpgradeType upgradeType, int cost, Sprite shopArt, Sprite toolbarArt, StatUpgrade statUpgrade) : this(name, description, upgradeType, cost, shopArt, toolbarArt)
    {
        this.statUpgrade = statUpgrade;
    }

    public Upgrade(string name, string description, UpgradeType upgradeType, int cost, Sprite shopArt, Sprite toolbarArt, ISkill skill) : this(name, description, upgradeType, cost, shopArt, toolbarArt)
    {
        this.skill = skill;
    }

    public Upgrade(string name, string description, UpgradeType upgradeType, int cost, Sprite shopArt, Sprite toolbarArt, PlayerModification playerMod) : this(name, description, upgradeType, cost, shopArt, toolbarArt)
    {
        this.playerMod = playerMod;
    }

    public Upgrade(string name, string description, UpgradeType upgradeType, int cost, Sprite shopArt, Sprite toolbarArt, GameModification gameMod) : this(name, description, upgradeType, cost, shopArt, toolbarArt)
    {
        this.gameMod = gameMod;
    }

    public Upgrade(string name, string description, UpgradeType upgradeType, int cost, Sprite shopArt, Sprite toolbarArt, Consumable consumable) : this(name, description, upgradeType, cost, shopArt, toolbarArt)
    {
        this.consumable = consumable;
    }
}
