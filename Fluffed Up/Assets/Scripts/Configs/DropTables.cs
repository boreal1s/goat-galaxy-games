using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTables : MonoBehaviour
{
    // Rates do not need to add to 1.0, instead they should be normalized before use.
    Dictionary<Upgrade.UpgradeType, float> upgradeTypeRates = new Dictionary<Upgrade.UpgradeType, float>()
    { 
        {Upgrade.UpgradeType.Skill, 0f }, // Temporarily 0 until we add skills, should be 0.2f
        {Upgrade.UpgradeType.SkillUpgrade, 0f }, // Temporarily 0 until we add skill upgrades, should be 0.2f
        {Upgrade.UpgradeType.StatUpgrade, 0.3f },
        {Upgrade.UpgradeType.PlayerModification, 0f }, // Temporarily 0 until we add skills, should be 0.15f
        {Upgrade.UpgradeType.GameModification, 0f }, // Temporarily 0 until we add skills, should be 0.15f
    };

    ArrayList statUpgrades = new ArrayList()
    {
        new StatUpgrade("Attack Power", "Increase attack power by 10", new ArrayList(), Upgrade.UpgradeType.StatUpgrade, 10, 1f, 10f)
    };
}
