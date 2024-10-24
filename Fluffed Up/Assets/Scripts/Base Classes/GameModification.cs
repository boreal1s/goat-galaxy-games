using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Upgrade;

public class GameModification : Upgrade
{
    public GameModification(Upgrade upgrade) : base(upgrade.name, upgrade.description, upgrade.followingUpgrades, upgrade.upgradeType, upgrade.cost)
    { }

    public GameModification(string name, string description, List<Upgrade> followingUpgrades, UpgradeType upgradeType, int cost) : base(name, description, followingUpgrades, upgradeType, cost)
    { }
}
