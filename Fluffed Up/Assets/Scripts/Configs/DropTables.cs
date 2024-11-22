using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KaimiraGames;
using Unity.VisualScripting;

public class DropTables : MonoBehaviour
{
    public enum Rarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Legendary = 3,
    }

    #region Artwork
    Sprite attackPowerShopSprite;
    Sprite attackPowerToolbarSprite;
    Sprite healthIncreaseShopSprite;
    Sprite healthIncreaseToolbarSprite;
    Sprite defenseUpgradeShopSprite;
    Sprite defenseUpgradeToolbarSprite;
    Sprite moveSpeedShopSprite;
    Sprite moveSpeedUpgradeToolbarSprite;
    Sprite attackSpeedUpgradeShopSprite;
    Sprite attackSpeedUpgradeToolbarSprite;
    Sprite dodgeSkillShopSprite;
    Sprite dodgeSkillToolbarSprite;
    Sprite blinkSkillShopSprite;
    Sprite blinkSkillToolbarSprite;
    Sprite fleshWoundModShopSprite;
    Sprite fleshWoundModToolbarSprite;
    Sprite restockModShopSprite;
    Sprite restockModToolbarSprite;
    Sprite healthPotionShopSprite;
    Sprite healthPotionToolbarSprite;
    #endregion

    #region Weights
    static int commonWeight = 40;
    static int uncommonWeight = 40;
    static int rareWeight = 40;
    static int legendaryWeight = 40;

    int skillWeight = 45;
    int statUpgradeWeight = 25;
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

    public WeightedList<UpgradeType> upgradeTypeTable;
    public WeightedList<Upgrade> skills;
    public WeightedList<Upgrade> statUpgrades;
    public WeightedList<Upgrade> playerModifications;
    public WeightedList<Upgrade> gameModifications;
    public WeightedList<Upgrade> consumables;
    public Dictionary<UpgradeType, int> availableUpgrades;

    void Awake()
    {
        #region Set Artwork
        attackPowerShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        attackPowerToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        healthIncreaseShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        healthIncreaseToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        defenseUpgradeShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        defenseUpgradeToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        moveSpeedShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        moveSpeedUpgradeToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        attackSpeedUpgradeShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        attackSpeedUpgradeToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        dodgeSkillShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        dodgeSkillToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        blinkSkillShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        blinkSkillToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        fleshWoundModShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        fleshWoundModToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        restockModShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        restockModToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        healthPotionShopSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        healthPotionToolbarSprite = Resources.Load<Sprite>("Skill Images/slay-the-spire-strike.png");
        #endregion

        upgradeTypeTable = new WeightedList<UpgradeType>
        {
            { UpgradeType.Skill, skillWeight },
            { UpgradeType.StatUpgrade, statUpgradeWeight },
            { UpgradeType.PlayerModification, playerModWeight },
            { UpgradeType.GameModification, gameModWeight }
        };
        Debug.Log("Upgrade type table populated");

        // Stat Upgrades
        Upgrade attackPower = new Upgrade("Attack Power", "Increase attack power by 10", UpgradeType.StatUpgrade, 20, attackPowerShopSprite, attackPowerToolbarSprite, new StatUpgrade(10f, StatType.AttackPower));
        Upgrade healthIncrease = new Upgrade("Max Health Increase", "Increase max health by 20", UpgradeType.StatUpgrade, 20, healthIncreaseShopSprite, healthIncreaseToolbarSprite, new StatUpgrade(20f, StatType.Health));
        Upgrade defenseUpgrade = new Upgrade("Defense Up!", "Become a little tougher", UpgradeType.StatUpgrade, 20, defenseUpgradeShopSprite, defenseUpgradeToolbarSprite, new StatUpgrade(2f, StatType.Defense));
        Upgrade moveSpeedUpgrade = new Upgrade("Move Speed Increase", "Zoooooom!", UpgradeType.StatUpgrade, 20, moveSpeedShopSprite, moveSpeedUpgradeToolbarSprite, new StatUpgrade(2.5f, StatType.MoveSpeed));
        Upgrade attackSpeedUpgrade = new Upgrade("Attack Speed Increase", "Increase attack speed by 7%", UpgradeType.StatUpgrade, 20, attackSpeedUpgradeShopSprite, attackSpeedUpgradeToolbarSprite, new StatUpgrade(0.07f, StatType.AttackSpeed));
        statUpgrades = new WeightedList<Upgrade>()
        {
            {attackPower, uncommonWeight},
            {healthIncrease, uncommonWeight},
            {defenseUpgrade, uncommonWeight},
            {moveSpeedUpgrade, uncommonWeight},
            {attackSpeedUpgrade, uncommonWeight},
        };

        // Skill
        Upgrade blinkSkill = new Upgrade("Blink", "Instantaneously teleport a short distance", UpgradeType.Skill, 200, blinkSkillShopSprite, blinkSkillToolbarSprite, new BlinkSkill(new List<Upgrade>(), Rarity.Rare, 4f, SkillType.Dodge));
        Upgrade dashSkill = new Upgrade("Dash", "A faster way to dodge", UpgradeType.Skill, 100, blinkSkillShopSprite, blinkSkillToolbarSprite, new DashSkill(new List<Upgrade>(), Rarity.Uncommon, 3f, SkillType.Dodge));
        Upgrade dodgeSkill = new Upgrade("Roll", "Swiftly roll out of harms way", UpgradeType.Skill, 55, dodgeSkillShopSprite, dodgeSkillToolbarSprite, new RollSkill(new List<Upgrade>() { blinkSkill, dashSkill }, Rarity.Common, 1.2f, SkillType.Dodge));
        skills = new WeightedList<Upgrade>()
        {
            { dodgeSkill, commonWeight},
        };

        // Player Modifications
        Upgrade fleshWoundMod = new Upgrade("Flesh Wound", "Occassionally negate a small amount of damage.", UpgradeType.PlayerModification, 175, fleshWoundModShopSprite, fleshWoundModToolbarSprite, new PlayerModification(new List<Upgrade>(), Rarity.Rare));
        playerModifications = new WeightedList<Upgrade>()
        {
            {fleshWoundMod, rareWeight},
        };

        // Game Modifications
        Upgrade restockMod = new Upgrade("Restock", "The shop now restocks on purchase.", UpgradeType.GameModification, 120, restockModShopSprite, restockModToolbarSprite, new GameModification(new List<Upgrade>(), Rarity.Rare));
        gameModifications = new WeightedList<Upgrade>()
        {
            {restockMod, rareWeight},
        };

        // Consumables
        Upgrade healthPotion = new Upgrade("Health Potion", "Heal a small amount of missing health.", UpgradeType.Consumable, 15, healthPotionShopSprite, healthPotionToolbarSprite, new Consumable(25f, ConsumableType.Health));
        consumables = new WeightedList<Upgrade>()
        {
            {healthPotion, commonWeight},
        };

        availableUpgrades = new Dictionary<UpgradeType, int>()
        {
            { UpgradeType.StatUpgrade, statUpgrades.Count},
            { UpgradeType.Skill, skills.Count},
            { UpgradeType.PlayerModification, playerModifications.Count},
            { UpgradeType.GameModification, gameModifications.Count},
            { UpgradeType.Consumable, consumables.Count},
        };
    }

    public UpgradeType getRandomUpgradeType()
    {
        Debug.Log("Getting random type");
        UpgradeType type = upgradeTypeTable.Next();
        Debug.Log($"Got random type {type}");
        if (availableUpgrades[type] < 1)
        {
            type = UpgradeType.StatUpgrade;
        }
        return type;
    }

    public Upgrade getRandomUpgrade(UpgradeType type)
    {
        while (availableUpgrades[type] == 0)
        {
            type = getRandomUpgradeType(); // Some types will never be empty so we shouldn't get stuck in an infinite loop
        }

        Debug.Log($"Got random type: {type}");
        Upgrade upgrade;

        switch (type)
        {
            case UpgradeType.StatUpgrade:
                upgrade = statUpgrades.Next();
                break;
            case UpgradeType.Skill:
                upgrade = skills.Next();
                skills.SetWeight(upgrade, 0);
                availableUpgrades[UpgradeType.Skill] -= 1;
                break;
            case UpgradeType.PlayerModification:
                upgrade = playerModifications.Next();
                playerModifications.SetWeight(upgrade, 0);
                availableUpgrades[UpgradeType.PlayerModification] -= 1;
                break;
            case UpgradeType.GameModification:
                upgrade = gameModifications.Next();
                gameModifications.SetWeight(upgrade, 0);
                availableUpgrades[UpgradeType.GameModification] -= 1;
                break;
            case UpgradeType.Consumable:
                upgrade = consumables.Next();
                break;
            default:
                Debug.Log("Uhm, this is awkward. getRandomWithReplacement was given an invalid argument.");
                return null;
        }

        return upgrade;
    }

    public void putBack(Upgrade upgrade)
    {
       
        switch (upgrade.upgradeType)
        {
            case UpgradeType.Skill:
                skills.SetWeight(upgrade, weightMap[upgrade.skill.GetRarity()]);
                availableUpgrades[UpgradeType.Skill] += 1;
                break;
            case UpgradeType.PlayerModification:
                playerModifications.SetWeight(upgrade, weightMap[upgrade.playerMod.rarity]);
                availableUpgrades[UpgradeType.PlayerModification] += 1;
                break;
            case UpgradeType.GameModification:
                Debug.Log($"{upgrade.upgradeName}");
                gameModifications.SetWeight(upgrade, weightMap[upgrade.gameMod.rarity]);
                availableUpgrades[UpgradeType.GameModification] += 1;
                break;
            default:
                break;
        }
    }

    public void purchase(Upgrade upgrade)
    {
        List<Upgrade> followingUpgrades;
        switch (upgrade.upgradeType)
        {
            case UpgradeType.StatUpgrade:
                statUpgrades[statUpgrades.IndexOf(upgrade)].cost = (int) Math.Round(statUpgrades[statUpgrades.IndexOf(upgrade)].cost * 1.5f);
                break;
            case UpgradeType.Skill:
                followingUpgrades = upgrade.skill.GetFollowingUprages();
                for (int i = 0; i < followingUpgrades.Count; i++)
                {
                    skills.Add(followingUpgrades[i], weightMap[followingUpgrades[i].skill.GetRarity()]);
                    availableUpgrades[upgrade.upgradeType] += 1;
                }
                break;
            case UpgradeType.PlayerModification:
                followingUpgrades = upgrade.playerMod.followingMods;
                for (int i = 0; i < followingUpgrades.Count; i++)
                {
                    playerModifications.Add(followingUpgrades[i], weightMap[followingUpgrades[i].playerMod.rarity]);
                    availableUpgrades[upgrade.upgradeType] += 1;
                }
                break;
            case UpgradeType.GameModification:
                followingUpgrades = upgrade.gameMod.followingMods;
                for (int i = 0; i < followingUpgrades.Count; i++)
                {
                    gameModifications.Add(followingUpgrades[i], weightMap[followingUpgrades[i].gameMod.rarity]);
                    availableUpgrades[upgrade.upgradeType] += 1;
                }
                break;
            default:
                break;
        }
    }
}
