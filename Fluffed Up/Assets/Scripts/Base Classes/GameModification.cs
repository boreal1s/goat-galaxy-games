using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Upgrade;

public class GameModification : Upgrade
{
    public GameModification(Upgrade upgrade) : base(upgrade.name, upgrade.description, upgrade.followingUpgrades, upgrade.upgradeType, upgrade.cost, upgrade.rarity)
    { }

    public GameModification(string name, string description, ArrayList followingUpgrades, UpgradeType upgradeType, int cost, float rarity) : base(name, description, followingUpgrades, upgradeType, cost, rarity)
    { }
}
