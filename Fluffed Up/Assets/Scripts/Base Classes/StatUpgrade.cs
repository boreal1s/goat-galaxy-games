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

    public StatUpgrade(Upgrade upgrade, float statValue) : base(upgrade.name, upgrade.description, upgrade.followingUpgrades, upgrade.upgradeType, upgrade.cost)
    {
        this.statValue = statValue;
    }

    public StatUpgrade(string name, string description, List<Upgrade> followingUpgrades, UpgradeType upgradeType, int cost, float statValue) : base(name, description, followingUpgrades, upgradeType, cost)
    {
        this.statValue = statValue;
    }
}
