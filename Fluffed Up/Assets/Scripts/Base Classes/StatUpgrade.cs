using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatUpgrade : Upgrade
{
    enum StatType
    {
        Health,
        Defense,
        AttackSpeed,
        Damage,
        MoveSpeed,
    }

    float statValue;

    public StatUpgrade(Upgrade upgrade, float statValue) : base(upgrade.name, upgrade.description, upgrade.followingUpgrades, upgrade.upgradeType, upgrade.cost, upgrade.shopArt, upgrade.toolbarArt)
    {
        this.statValue = statValue;
    }

    public StatUpgrade(string name, string description, List<Upgrade> followingUpgrades, UpgradeType upgradeType, int cost, float statValue, Sprite shopArt, Sprite toolbarArt) : base(name, description, followingUpgrades, upgradeType, cost, shopArt, toolbarArt)
    {
        this.statValue = statValue;
    }
}
