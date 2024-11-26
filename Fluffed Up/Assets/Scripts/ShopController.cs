using Cinemachine;
using System.Collections;
using TMPro;
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

    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip purchaseSound; 

    private Upgrade upgrade1;
    private Upgrade upgrade2;
    private Upgrade upgrade3;
    private Upgrade consumable;

    private bool upgrade1Purchased;
    private bool upgrade2Purchased;
    private bool upgrade3Purchased;
    private bool consumablePurchased;

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
        if (canStock)
            StockShop();
    }

    public void OpenShop()
    {
        if (canTriggerShop)
        {
            bgmPlayer.DimAndDull();
            Debug.Log("Shop opened");
            activateShop();
            shopIsOpen = true;

            if (player != null)
                player.enabled = false;

            if (freeLookCamera != null)
                freeLookCamera.enabled = false;
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
            player.enabled = true;

        if (freeLookCamera != null)
            freeLookCamera.enabled = true;
    }

    public void RestockShop()
    {
        if (!upgrade1Purchased && upgrade1 != null)
            dropTables.putBack(upgrade1);

        if (!upgrade2Purchased && upgrade2 != null)
            dropTables.putBack(upgrade2);

        if (!upgrade3Purchased && upgrade3 != null)
            dropTables.putBack(upgrade3);

        if (!consumablePurchased && consumable != null)
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

    private void StockShop()
    {
        Debug.Log("Stocking shop");
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
        switch (i)
        {
            case 1:
                if (player.GetCoins() >= upgrade1.cost)
                {
                    player.UpdateCoins(-upgrade1.cost);
                    option1.SetActive(false);
                    dropTables.purchase(upgrade1);
                    upgrade1Purchased = true;
                    HandleUpgrade(upgrade1);
                    PlayPurchaseSound();
                }
                break;
            case 2:
                if (player.GetCoins() >= upgrade2.cost)
                {
                    player.UpdateCoins(-upgrade2.cost);
                    option2.SetActive(false);
                    dropTables.purchase(upgrade2);
                    upgrade2Purchased = true;
                    HandleUpgrade(upgrade2);
                    PlayPurchaseSound();
                }
                break;
            case 3:
                if (player.GetCoins() >= upgrade3.cost)
                {
                    player.UpdateCoins(-upgrade3.cost);
                    option3.SetActive(false);
                    dropTables.purchase(upgrade3);
                    upgrade3Purchased = true;
                    HandleUpgrade(upgrade3);
                    PlayPurchaseSound();
                }
                break;
            case 4:
                if (player.GetCoins() >= consumable.cost)
                {
                    player.UpdateCoins(-consumable.cost);
                    consumableOption.SetActive(false);
                    dropTables.purchase(consumable);
                    consumablePurchased = true;
                    HandleUpgrade(consumable);
                    PlayPurchaseSound();
                }
                break;
        }
    }

    private void PlayPurchaseSound()
    {
        if (audioSource != null && purchaseSound != null)
        {
            audioSource.PlayOneShot(purchaseSound);
        }
        else
        {
            Debug.LogWarning("AudioSource or PurchaseSound not set!");
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
            player.dodgeSkill = skill;
        }
    }

    private void HandlePlayerModification(PlayerModification mod) { }

    private void HandleGameModification(GameModification mod) { }

    private void HandleConsumablePurchase(Consumable consumable) { }
}
