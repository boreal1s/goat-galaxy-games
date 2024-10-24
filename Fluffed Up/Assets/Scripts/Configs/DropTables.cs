using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTables : MonoBehaviour
{

    [SerializeField]
    CharacterClass player;

    public enum Rarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Legendary = 3,
    }

    public Dictionary<Upgrade.UpgradeType, float> upgradeTypeRates;
    public Dictionary<Rarity, float> rarityRates;
    public Dictionary<Rarity, HashSet<Skill>> skills;
    public Dictionary<Rarity, HashSet<StatUpgrade>> statUpgrades;
    public Dictionary<Rarity, HashSet<PlayerModification>> playerModifications;
    public Dictionary<Rarity, HashSet<GameModification>> gameModifications;
    public Dictionary<Rarity, HashSet<Consumable>> consumables;

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

        // Rates do not need to add to 1.0, instead they should be normalized before use.
        rarityRates = new Dictionary<Rarity, float>()
        {
            {Rarity.Common, 0.5f }, // Temporarily 0 until we add skills, should be 0.2f
            {Rarity.Uncommon, 0.35f },
            {Rarity.Rare, 0.12f }, // Temporarily 0 until we add skills, should be 0.15f
            {Rarity.Legendary, 0.03f }, // Temporarily 0 until we add skills, should be 0.15f
        };

        // Stat Upgrades
        StatUpgrade attackPower = new StatUpgrade("Attack Power", "Increase attack power by 10", new List<Upgrade>(), Upgrade.UpgradeType.StatUpgrade, 10, 10f);
        StatUpgrade healthIncrease = new StatUpgrade("Max Health Increase", "Increase max health by 10", new List<Upgrade>(), Upgrade.UpgradeType.StatUpgrade, 10, 10f);
        statUpgrades = new Dictionary<Rarity, List<StatUpgrade>>()
        {
            {Rarity.Uncommon, new HashSet<StatUpgrade>(){attackPower, healthIncrease} },
        };

        // Skill
        Skill blinkSkill = new Skill("Blink", "Instantaneously teleport a short distance", new List<Upgrade>(), Upgrade.UpgradeType.Skill, 20, 0, 5);
        Skill dodgeSkill = new Skill("Roll", "Swiftly roll out of harms way", new List<Upgrade>(){ blinkSkill }, Upgrade.UpgradeType.Skill, 10, 0, 5);
        skills = new Dictionary<Rarity, List<Skill>>()
        {
             {Rarity.Common, new HashSet<Skill>(){ dodgeSkill } },
        };
        
        // Player Modifications
        // Occasionally negate damage

        // Game Modifications
        // Rare items are more common 
    }
}
