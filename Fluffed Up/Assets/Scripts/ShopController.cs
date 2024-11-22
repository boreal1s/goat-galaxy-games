using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Remoting.Metadata;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] private GameObject shopComponent;
    [SerializeField] private GameObject dt;
    private PlayerController player;
    private CinemachineFreeLook freeLookCamera;
    private BGMPlayer bgmPlayer;

    [SerializeField] private GameObject option1;
    [SerializeField] private GameObject option1ShopImage;
    [SerializeField] private GameObject option1Name;
    [SerializeField] private GameObject option1Description;

    [SerializeField] private GameObject option2;
    [SerializeField] private GameObject option2ShopImage;
    [SerializeField] private GameObject option2Name;
    [SerializeField] private GameObject option2Description;

    [SerializeField] private GameObject option3;
    [SerializeField] private GameObject option3ShopImage;
    [SerializeField] private GameObject option3Name;
    [SerializeField] private GameObject option3Description;

    [SerializeField] private GameObject consumableOption;
    [SerializeField] private GameObject consumableOptionShopImage;
    [SerializeField] private GameObject consumableOptionName;
    [SerializeField] private GameObject consumableOptionDescription;

    Upgrade upgrade1;
    Upgrade upgrade2;
    Upgrade upgrade3;
    Upgrade consumable;

    bool upgrade1Purchased;
    bool upgrade2Purchased;
    bool upgrade3Purchased;
    bool consumablePurchased;

    public bool shopIsOpen;
    public bool canTriggerShop = false;
    public bool canStock = false;
    private DropTables dropTables;

    private void Start()
    {
        shopIsOpen = false;
        shopComponent.SetActive(false);
        dropTables = dt.GetComponent<DropTables>();
        Debug.Log($"DropTable Found: {dropTables}");

        player = FindObjectOfType<PlayerController>();
        freeLookCamera = FindObjectOfType<CinemachineFreeLook>();

        GameObject bgmObject = GameObject.FindGameObjectWithTag("BGM");
        bgmPlayer = bgmObject.GetComponent<BGMPlayer>();
    }

    private void Update()
    {
        if (shopComponent.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (canStock == true)
            StockShop();
    }

    public void OpenShop()
    {
        if (canTriggerShop)
        {
            bgmPlayer.DimAndDull();
            Debug.Log("Shop opened");
            activateShop();
            Debug.Log($"Shop component: {shopComponent}");
            shopIsOpen = true;

            if (player != null)
            {
                player.enabled = false; // Disable player movement
            }

            if (freeLookCamera != null)
            {
                freeLookCamera.enabled = false; // Disable the CinemachineFreeLook component
            }
        }
    }

    public void CloseShop()
    {
        bgmPlayer.LoudAndClear();
        shopComponent.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        shopIsOpen = false;

        if (player != null)
            player.enabled = true; // Re-enable player movement

        if (freeLookCamera != null)
            freeLookCamera.enabled = true; // Re-enable the CinemachineFreeLook component
    }

    public void RestockShop()
    {
        if (upgrade1Purchased == false && upgrade1 != null)
            dropTables.putBack(upgrade1);

        if (upgrade2Purchased == false && upgrade2 != null)
            dropTables.putBack(upgrade2);

        if (upgrade3Purchased == false && upgrade3 != null)
            dropTables.putBack(upgrade3);

        if (consumablePurchased == false && consumable != null) // Should never actually do anything unless we make consumables limited in quantity
            dropTables.putBack(consumable);

        canStock = true;
    }

    private void activateShop()
    {
        shopComponent.SetActive(true);
        Time.timeScale = 0;

        if (!upgrade1Purchased)
        {
            option1.SetActive(true);
            option1ShopImage.GetComponent<Image>().overrideSprite = upgrade1.shopArt;
            option1Name.GetComponent<TextMeshProUGUI>().text = upgrade1.upgradeName;
            option1Description.GetComponent<TextMeshProUGUI>().text = upgrade1.description;
        }

        if (!upgrade2Purchased)
        {
            option2.SetActive(true);
            option2ShopImage.GetComponent<Image>().overrideSprite = upgrade2.shopArt;
            option2Name.GetComponent<TextMeshProUGUI>().text = upgrade2.upgradeName;
            option2Description.GetComponent<TextMeshProUGUI>().text = upgrade2.description;
        }

        if (!upgrade3Purchased)
        {
            option3.SetActive(true);
            option3ShopImage.GetComponent<Image>().overrideSprite = upgrade3.shopArt;
            option3Name.GetComponent<TextMeshProUGUI>().text = upgrade3.upgradeName;
            option3Description.GetComponent<TextMeshProUGUI>().text = upgrade3.description;
        }

        if (!consumablePurchased)
        {
            consumableOption.SetActive(true);
            consumableOptionShopImage.GetComponent<Image>().overrideSprite = consumable.shopArt;
            consumableOptionName.GetComponent<TextMeshProUGUI>().text = consumable.upgradeName;
            consumableOptionDescription.GetComponent<TextMeshProUGUI>().text = consumable.description;
        }

        Debug.Log("Shop component activated");
    }

    private void StockShop() {
        Debug.Log("Stocking shop");
        Debug.Log($"Option 1: {option1}");
        Debug.Log($"DropTables: {dropTables}");
        upgrade1 = dropTables.getRandomUpgrade(dropTables.getRandomUpgradeType());
        upgrade1Purchased = false;
        upgrade2 = dropTables.getRandomUpgrade(dropTables.getRandomUpgradeType());
        upgrade2Purchased = false;
        upgrade3 = dropTables.getRandomUpgrade(dropTables.getRandomUpgradeType());
        upgrade3Purchased = false;
        consumable = dropTables.getRandomUpgrade(UpgradeType.Consumable);
        consumablePurchased = false;
        canStock = false;
        Debug.Log($"Stocked with:  {upgrade1.upgradeName}, {upgrade2.upgradeName}, {upgrade3.upgradeName}, {consumable.upgradeName}");
    }

    public void BuyOption(int i)
    {
        switch (i) {
            case 1:
                if (player.GetCoins() >= upgrade1.cost)
                {
                    player.UpdateCoins(-upgrade1.cost);
                    Debug.Log($"Buying: {upgrade1.upgradeName}");
                    option1.SetActive(false);
                    dropTables.purchase(upgrade1);
                    upgrade1Purchased = true;
                    HandleUpgrade(upgrade1);
                }
                else
                {
                    Debug.Log("Not enough money");
                }
                break;
            case 2:
                if (player.GetCoins() >= upgrade2.cost)
                {
                    player.UpdateCoins(-upgrade2.cost);
                    Debug.Log($"Buying: {upgrade2.upgradeName}");
                    option2.SetActive(false);
                    dropTables.purchase(upgrade2);
                    upgrade2Purchased = true;
                    HandleUpgrade(upgrade2);
                }
                else
                {
                    Debug.Log("Not enough money");
                }
                break;
            case 3:
                if (player.GetCoins() >= upgrade3.cost)
                {
                    player.UpdateCoins(-upgrade3.cost);
                    Debug.Log($"Buying: {upgrade3.upgradeName}");
                    option3.SetActive(false);
                    dropTables.purchase(upgrade3);
                    upgrade3Purchased = true;
                    HandleUpgrade(upgrade3);
                }
                else
                {
                    Debug.Log("Not enough money");
                }
                break;
            case 4:
                if (player.GetCoins() >= consumable.cost)
                {
                    player.UpdateCoins(-consumable.cost);
                    Debug.Log($"Buying: {consumable.upgradeName}");
                    consumableOption.SetActive(false);
                    dropTables.purchase(consumable);
                    consumablePurchased = true;
                    HandleUpgrade(consumable);
                }
                else
                {
                    Debug.Log("Not enough money");
                }
                break;
            default: 
                break;
        }
    }

    private void HandleUpgrade(Upgrade upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.StatUpgrade:
                HandleStatUpgrade(upgrade.statUpgrade);
                break;
            case UpgradeType.Skill:
                HandleSkillUpgrade(upgrade.skill);
                break;
            case UpgradeType.PlayerModification:
                HandlePlayerModification(upgrade.playerMod);
                break;
            case UpgradeType.GameModification:
                HandleGameModification(upgrade.gameMod);
                break;
            case UpgradeType.Consumable:
                HandleConsumablePurchase(upgrade.consumable);
                break;
            default:
                break;
        }
    }

    private void HandleStatUpgrade(StatUpgrade statUpgrade)
    {
        switch (statUpgrade.statType)
        {
            case StatType.Health:
                player.updateMaxHealth(statUpgrade.statValue);
                break;
            case StatType.Defense:
                player.defense += statUpgrade.statValue;
                break;
            case StatType.AttackSpeed:
                player.UpdateAttackSpeed(statUpgrade.statValue);
                break;
            case StatType.AttackPower:
                player.attackPower += statUpgrade.statValue;
                player.projectileDamage += (statUpgrade.statValue * .4f);
                break;
            case StatType.MoveSpeed:
                player.moveSpeed += statUpgrade.statValue;
                break;
        }
    }

    private void HandleSkillUpgrade(ISkill skill)
    {
        skill.SetCharacter(ref player);
        if (skill.GetSkillType() == SkillType.Dodge)
        {
            Debug.Log("Dodge purchased");
            player.dodgeSkill = skill;
        }
    }

    private void HandlePlayerModification(PlayerModification mod)
    {
        return;
    }

    private void HandleGameModification (GameModification modl)
    {
        return;
    }

    private void HandleConsumablePurchase(Consumable consumable)
    {
        return;
    }
}
