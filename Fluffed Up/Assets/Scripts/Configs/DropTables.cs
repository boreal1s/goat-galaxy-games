using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KaimiraGames;
using Unity.VisualScripting;

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

    #region Artwork
    [SerializeField] Sprite attackPowerShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite attackPowerToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite healthIncreaseShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite healthIncreaseToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite dodgeSkillShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite dodgeSkillToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite blinkSkillShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite blinkSkillToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite fleshWoundModShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite fleshWoundModToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite restockModShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite restockModToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite healthPotionShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    [SerializeField] Sprite healthPotionToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
    #endregion

    #region Weights
    static int commonWeight = 40;
    static int uncommonWeight = 40;
    static int rareWeight = 40;
    static int legendaryWeight = 40;

    int skillWeight = 40;
    int statUpgradeWeight = 30;
    int playerModWeight = 15;
    int gameModWeight = 15;

    public Dictionary<Rarity, int> weightMap = new Dictionary<Rarity, int>()
    {
        { Rarity.Common, commonWeight },
        { Rarity.Uncommon, uncommonWeight },
        { Rarity.Rare, rareWeight },
        { Rarity.Legendary, legendaryWeight },
    };
    #endregion

    public WeightedList<Upgrade.UpgradeType> upgradeTypeTable;
    public WeightedList<Skill> skills;
    public WeightedList<StatUpgrade> statUpgrades;
    public WeightedList<PlayerModification> playerModifications;
    public WeightedList<GameModification> gameModifications;
    public WeightedList<Consumable> consumables;
    public Dictionary<Upgrade.UpgradeType, int> availableUpgrades;

    public DropTables()
    {
        if (player == null)
            Debug.Log("DropTables instance is missing attached character class");

        upgradeTypeTable = new WeightedList<Upgrade.UpgradeType>
        {
            { Upgrade.UpgradeType.Skill, skillWeight },
            { Upgrade.UpgradeType.StatUpgrade, statUpgradeWeight },
            { Upgrade.UpgradeType.PlayerModification, playerModWeight },
            { Upgrade.UpgradeType.GameModification, gameModWeight }
        };

        // Stat Upgrades
        StatUpgrade attackPower = new StatUpgrade("Attack Power", "Increase attack power by 10", new List<Upgrade>(), Upgrade.UpgradeType.StatUpgrade, 10, 10f, attackPowerShopSprite, attackPowerToolbarSprite);
        StatUpgrade healthIncrease = new StatUpgrade("Max Health Increase", "Increase max health by 10", new List<Upgrade>(), Upgrade.UpgradeType.StatUpgrade, 10, 10f, healthPotionShopSprite, healthPotionToolbarSprite);
        statUpgrades = new WeightedList<StatUpgrade>()
        {
            {attackPower, uncommonWeight},
            {healthIncrease, uncommonWeight},
        };

        // Skill
        Skill blinkSkill = new Skill("Blink", "Instantaneously teleport a short distance", new List<Skill>(), Upgrade.UpgradeType.Skill, 20, 0, 5, Rarity.Rare, blinkSkillShopSprite, blinkSkillToolbarSprite);
        Skill dodgeSkill = new Skill("Roll", "Swiftly roll out of harms way", new List<Skill>(){ blinkSkill }, Upgrade.UpgradeType.Skill, 10, 0, 5, Rarity.Common, dodgeSkillShopSprite, dodgeSkillToolbarSprite);
        skills = new WeightedList<Skill>()
        {
            { dodgeSkill, commonWeight},
        };

        // Player Modifications
        PlayerModification fleshWoundMod = new PlayerModification("Flesh Wound", "Occassionally negate a small amount of damage.", new List<PlayerModification>(), Upgrade.UpgradeType.PlayerModification, 20, Rarity.Rare, fleshWoundModShopSprite, fleshWoundModToolbarSprite);
        playerModifications = new WeightedList<PlayerModification>()
        {
            {fleshWoundMod, rareWeight},
        };

        // Game Modifications
        GameModification restockMod = new GameModification("Restock", "The shop now restocks after a purchase is made.", new List<GameModification>(), Upgrade.UpgradeType.GameModification, 20, Rarity.Rare, restockModShopSprite, restockModToolbarSprite);
        gameModifications = new WeightedList<GameModification>()
        {
            {restockMod, uncommonWeight},
        };

        // Consumables
        Consumable healthPotion = new Consumable("Health Potion", "Regain some missing health.", 20, 25, healthPotionShopSprite, healthPotionToolbarSprite);
        consumables = new WeightedList<Consumable>()
        {
            {healthPotion, commonWeight},
        };

        availableUpgrades = new Dictionary<Upgrade.UpgradeType, int>()
        {
            { Upgrade.UpgradeType.StatUpgrade, statUpgrades.Count},
            { Upgrade.UpgradeType.Skill, skills.Count},
            { Upgrade.UpgradeType.PlayerModification, playerModifications.Count},
            { Upgrade.UpgradeType.GameModification, gameModifications.Count},
        };
    }

    public Upgrade.UpgradeType getRandomUpgradeType()
    {
        Upgrade.UpgradeType type = upgradeTypeTable.Next();
        if (availableUpgrades[type] < 1)
        {
            type = Upgrade.UpgradeType.StatUpgrade;
        }
        return type;
    }

    public StatUpgrade getRandomStatUpgrade()
    {
        if (statUpgrades.Count == 0)
        {
            upgradeTypeTable.SetWeight(Upgrade.UpgradeType.StatUpgrade, 0);
            return null;
        }

        return statUpgrades.Next();
    }

    public dynamic getRandomUniqueUpgrade(Upgrade.UpgradeType type)
    {
        dynamic upgradeList;
        switch (type)
        {
            case Upgrade.UpgradeType.Skill:
                upgradeList = skills;
                break;
            case Upgrade.UpgradeType.PlayerModification:
                upgradeList = playerModifications;
                break;
            case Upgrade.UpgradeType.GameModification:
                upgradeList = gameModifications;
                break;
            default:
                Debug.Log("Uhm, this is awkward. getRandomUniqueUpgrade was given an invalid argument.");
                return null;
        }

        if (0 < upgradeList.Count)
        {
            upgradeTypeTable.SetWeight(type, 0);
            return null;
        }

        dynamic upgrade = upgradeList.Next();
        upgradeList.SetWeight(upgrade, 0);
        availableUpgrades[type] -= 1;

        for (int i = 0; i < upgrade.followingUpgrades.Count; i++)
        {
            skills.Add(upgrade.followingUpgrades[i], weightMap[upgrade.followingUpgrades[i].rarity]);
            availableUpgrades[type] += 1;
        }

        if (availableUpgrades[type] == 0)
            upgradeTypeTable.SetWeight(type, 0);

        return upgrade;
    }
}
