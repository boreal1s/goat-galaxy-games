using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModification : Upgrade
{
    public PlayerModification(Upgrade upgrade) : base(upgrade.name, upgrade.description, upgrade.followingUpgrades, upgrade.upgradeType, upgrade.cost)
    {}

    public PlayerModification(string name, string description, List<Upgrade> followingUpgrades, UpgradeType upgradeType, int cost) : base(name, description, followingUpgrades, upgradeType, cost)
    {}
}
