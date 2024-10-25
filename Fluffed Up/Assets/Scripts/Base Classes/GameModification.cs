using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Upgrade;

public class GameModification : Upgrade
{
    public List<GameModification> followingUpgrades;
    public DropTables.Rarity rarity;
    public GameModification(Upgrade upgrade, List<GameModification> followingUpgrades, DropTables.Rarity rarity) : base(upgrade.name, upgrade.description, upgrade.upgradeType, upgrade.cost, upgrade.shopArt, upgrade.toolbarArt)
    {
        this.followingUpgrades = followingUpgrades;
        this.rarity = rarity;
    }

    public GameModification(string name, string description, List<GameModification> followingUpgrades, UpgradeType upgradeType, int cost, DropTables.Rarity rarity, Sprite shopArt, Sprite toolbarArt) : base(name, description, upgradeType, cost, shopArt, toolbarArt)
    {
        this.followingUpgrades = followingUpgrades;
        this.rarity = rarity;
    }
}
