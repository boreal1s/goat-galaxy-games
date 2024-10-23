using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModification : Upgrade
{
    public PlayerModification(Upgrade upgrade) : base(upgrade.name, upgrade.description, upgrade.followingUpgrades, upgrade.upgradeType, upgrade.cost, upgrade.rarity)
    {}

    public PlayerModification(string name, string description, ArrayList followingUpgrades, UpgradeType upgradeType, int cost, float rarity) : base(name, description, followingUpgrades, upgradeType, cost, rarity)
    {}
}
