using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModification : Upgrade
{
    public List<PlayerModification> followingMods;
    public DropTables.Rarity rarity;

    public PlayerModification(Upgrade upgrade, List<PlayerModification> followingMods, DropTables.Rarity rarity) : base(upgrade.name, upgrade.description, upgrade.upgradeType, upgrade.cost, upgrade.shopArt, upgrade.toolbarArt)
    {
        this.followingMods = followingMods;
        this.rarity = rarity;
    }

    public PlayerModification(string name, string description, List<PlayerModification> followingMods, UpgradeType upgradeType, int cost, DropTables.Rarity rarity, Sprite shopArt, Sprite toolbarArt) : base(name, description, upgradeType, cost, shopArt, toolbarArt)
    {
        this.followingMods = followingMods;
        this.rarity = rarity;
    }
}
