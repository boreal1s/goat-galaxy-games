using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController: MonoBehaviour
{
    [SerializeField] private GameObject shopComponent;
    [SerializeField] private GameObject dt;
    [SerializeField] private GameObject uf;
    private PlayerController player;
    private GameObject virtualCameras;
    private BGMPlayer bgmPlayer;

    [Header("Option 1 UI Elements")]
    [SerializeField] private GameObject option1;
    [SerializeField] private GameObject option1ShopImage;
    [SerializeField] private GameObject option1Name;
    [SerializeField] private GameObject option1Description;
    [SerializeField] private GameObject notEnoughCoinsText1; // Text for option 1

    [Header("Option 2 UI Elements")]
    [SerializeField] private GameObject option2;
    [SerializeField] private GameObject option2ShopImage;
    [SerializeField] private GameObject option2Name;
    [SerializeField] private GameObject option2Description;
    [SerializeField] private GameObject notEnoughCoinsText2; // Text for option 2

    [Header("Option 3 UI Elements")]
    [SerializeField] private GameObject option3;
    [SerializeField] private GameObject option3ShopImage;
    [SerializeField] private GameObject option3Name;
    [SerializeField] private GameObject option3Description;
    [SerializeField] private GameObject notEnoughCoinsText3; // Text for option 3

    [Header("Consumable UI Elements")]
    [SerializeField] private GameObject consumableOption;
    [SerializeField] private GameObject consumableOptionShopImage;
    [SerializeField] private GameObject consumableOptionName;
    [SerializeField] private GameObject consumableOptionDescription;
    [SerializeField] private GameObject notEnoughCoinsText4; // Text for consumable option

    [Header("Sound Effects")]
    [SerializeField]
    public AudioClip coinJiggleSound;
    public AudioClip coinLowSound;
    private AudioSource audioSource;        // Reference to AudioSource
    private AudioClip confirmSound;        // Sound for successful purchase

    Upgrade upgrade1;
    Upgrade upgrade2;
    Upgrade upgrade3;
    Upgrade consumable;

    private bool upgrade1Purchased;
    private bool upgrade2Purchased;
    private bool upgrade3Purchased;
    private bool consumablePurchased;

    public bool shopIsOpen;
    public bool canTriggerShop = false;
    public bool canStock = false;
    private DropTables dropTables;
    public UpgradeFlags upgradeFlags;

    private void Start()
    {
        shopIsOpen = false;
        shopComponent.SetActive(false);

        // Ensure all "Not Enough Coins" messages are hidden initially
        notEnoughCoinsText1.SetActive(false);
        notEnoughCoinsText2.SetActive(false);
        notEnoughCoinsText3.SetActive(false);
        notEnoughCoinsText4.SetActive(false);

        dropTables = dt.GetComponent<DropTables>();
        player = FindObjectOfType<PlayerController>();
        virtualCameras = GameObject.Find("VirtualCameras");

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

        player.isShopping = shopIsOpen;
    }

    public void OpenShop()
    {
        if (canTriggerShop)
        {
            bgmPlayer.DimAndDull();
            shopComponent.SetActive(true);
            shopIsOpen = true;
            Time.timeScale = 0;

            if (player != null)
                player.enabled = false;
\
            if (virtualCameras != null)
            {
                virtualCameras.SetActive(false);
            }
        }
    }

    public void CloseShop()
    {
        bgmPlayer.LoudAndClear();
        shopComponent.SetActive(false);

        // Hide all "Not Enough Coins" messages when the shop closes
        notEnoughCoinsText1.SetActive(false);
        notEnoughCoinsText2.SetActive(false);
        notEnoughCoinsText3.SetActive(false);
        notEnoughCoinsText4.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        shopIsOpen = false;

        if (player != null)
            player.enabled = true;

        if (virtualCameras != null)
            virtualCameras.SetActive(true);
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
            loadShopComponent(1);
        }

        if (!upgrade2Purchased)
        {
            loadShopComponent(2);
        }

        if (!upgrade3Purchased)
        {
            loadShopComponent(3);
        }

        if (!consumablePurchased)
        {
            loadShopComponent(4);
        }

        Debug.Log("Shop component activated");
    }

    private void loadShopComponent(int shopNumber)
    {
        if (shopNumber == 1)
        {
            option1.SetActive(true);
            option1ShopImage.GetComponent<Image>().overrideSprite = upgrade1.shopArt;
            option1Name.GetComponent<TextMeshProUGUI>().text = upgrade1.upgradeName;
            option1Description.GetComponent<TextMeshProUGUI>().text = upgrade1.description;
        }

        if (shopNumber == 2)
        {
            option2.SetActive(true);
            option2ShopImage.GetComponent<Image>().overrideSprite = upgrade2.shopArt;
            option2Name.GetComponent<TextMeshProUGUI>().text = upgrade2.upgradeName;
            option2Description.GetComponent<TextMeshProUGUI>().text = upgrade2.description;
        }

        if (shopNumber == 3)
        {
            option3.SetActive(true);
            option3ShopImage.GetComponent<Image>().overrideSprite = upgrade3.shopArt;
            option3Name.GetComponent<TextMeshProUGUI>().text = upgrade3.upgradeName;
            option3Description.GetComponent<TextMeshProUGUI>().text = upgrade3.description;
        }

        if (shopNumber == 4)
        {
            consumableOption.SetActive(true);
            consumableOptionShopImage.GetComponent<Image>().overrideSprite = consumable.shopArt;
            consumableOptionName.GetComponent<TextMeshProUGUI>().text = consumable.upgradeName;
            consumableOptionDescription.GetComponent<TextMeshProUGUI>().text = consumable.description;
        }
    }


    private void StockShop()
    {
        upgrade1 = dropTables.getRandomUpgrade(dropTables.getRandomUpgradeType());
        upgrade1Purchased = false;
        upgrade2 = dropTables.getRandomUpgrade(dropTables.getRandomUpgradeType());
        upgrade2Purchased = false;
        upgrade3 = dropTables.getRandomUpgrade(dropTables.getRandomUpgradeType());
        upgrade3Purchased = false;
        consumable = dropTables.getRandomUpgrade(UpgradeType.Consumable);
        consumablePurchased = false;
        canStock = false;
    }

    public void BuyOption(int i)
    {
        switch (i)
        {
            case 1:
                AttemptPurchase(upgrade1, ref upgrade1Purchased, option1, notEnoughCoinsText1);
                break;
            case 2:
                AttemptPurchase(upgrade2, ref upgrade2Purchased, option2, notEnoughCoinsText2);
                break;
            case 3:
                AttemptPurchase(upgrade3, ref upgrade3Purchased, option3, notEnoughCoinsText3);
                break;
            case 4:
                AttemptPurchase(consumable, ref consumablePurchased, consumableOption, notEnoughCoinsText4);
                break;
            default:
                break;
        }
    }

    private void AttemptPurchase(Upgrade upgrade, ref bool isPurchased, GameObject option, GameObject notEnoughCoinsText)
    {
        if (player.GetCoins() >= upgrade.cost)
        {
            player.UpdateCoins(-upgrade.cost);
            option.SetActive(false);
            dropTables.purchase(upgrade);
            isPurchased = true;
            HandleUpgrade(upgrade);
            PlayConfirmSound();
        }
        else
        {
            notEnoughCoinsText.SetActive(true); // Show specific item's message
        }
    }

    private void PlayConfirmSound()
    {
        if (audioSource != null && confirmSound != null)
            audioSource.PlayOneShot(confirmSound);
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
            case UpgradeType.PlayerModification: // fall-through
            case UpgradeType.GameModification:
                HandleModification(upgrade);
                break;
            case UpgradeType.Consumable:
                HandleConsumablePurchase(upgrade);
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
        dropTables.removeSkillSiblings(skill.GetSkillType(), skill.GetFollowingUprages());

        skill.SetCharacter(ref player);
        if (skill.GetSkillType() == SkillType.Dodge)
        {
            player.dodgeSkill = skill;
        }
    }

    private void HandleModification(Upgrade mod)
    {
        upgradeFlags.setUpgradeFlag(mod.upgradeName, mod); ;
    }

    private void HandleConsumablePurchase(Upgrade upgrade)
    {
        player.CollectItem(new CollectibleItem(upgrade.upgradeName, (int)upgrade.consumable.power, 1, (int)upgrade.consumable.power));
    }
}
