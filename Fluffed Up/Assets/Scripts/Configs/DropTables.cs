using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTables : MonoBehaviour
{

    [SerializeField]
    CharacterClass player;

    public Dictionary<Upgrade.UpgradeType, float> upgradeTypeRates;
    public Dictionary<Skill, float> skills;
    public Dictionary<StatUpgrade, float> statUpgrades;
    public Dictionary<PlayerModification, float> playerModifications;
    public Dictionary<GameModification, float> gameModifications;

    public DropTables()
    {
        if (player == null)
            Debug.Log("DropTables instance is missing attached character class");

        // Rates do not need to add to 1.0, instead they should be normalized before use.
        upgradeTypeRates = new Dictionary<Upgrade.UpgradeType, float>()
        {
            {Upgrade.UpgradeType.Skill, 0f }, // Temporarily 0 until we add skills, should be 0.2f
            {Upgrade.UpgradeType.StatUpgrade, 0.3f },
            {Upgrade.UpgradeType.PlayerModification, 0f }, // Temporarily 0 until we add skills, should be 0.15f
            {Upgrade.UpgradeType.GameModification, 0f }, // Temporarily 0 until we add skills, should be 0.15f
        };

        StatUpgrade attackPower = new StatUpgrade("Attack Power", "Increase attack power by 10", new ArrayList(), Upgrade.UpgradeType.StatUpgrade, 10, 1f, 10f);
        StatUpgrade healthIncrease = new StatUpgrade("Max Health Increase", "Increase max health by 10", new ArrayList(), Upgrade.UpgradeType.StatUpgrade, 10, 1f, 10f);
        statUpgrades = new Dictionary<StatUpgrade, float>()
        {
            {attackPower, 50f},
            {healthIncrease, 50f}
        };

        

    }
}
