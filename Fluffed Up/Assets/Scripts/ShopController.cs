using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] private GameObject shopComponent;
    [SerializeField] private GameObject dt;

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

    public bool shopIsOpen;
    public bool shopIsStocked;
    private DropTables dropTables;

    private void Start()
    {
        shopComponent.SetActive(false);
        dropTables = dt.GetComponent<DropTables>();
        Debug.Log($"DropTable Found: {dropTables}");
        shopIsStocked = false;
    }

    private void Update()
    {
        if (shopComponent.activeSelf)
            Cursor.lockState = CursorLockMode.None;
        if (shopIsStocked == false)
            StockShop();
    }

    public void OpenShop()
    {
        Debug.Log("Shop opened");
        activateShop();
        Debug.Log($"Shop component: {shopComponent}");
        Time.timeScale = 0;
        shopIsOpen = true;
    }

    public void CloseShop()
    {
        shopComponent.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        shopIsOpen = false;
        shopIsStocked = false;
    }

    private void activateShop()
    {
        shopComponent.SetActive(true);
        option1.SetActive(true);
        option2.SetActive(true);
        option3.SetActive(true);
        consumableOption.SetActive(true);

        option1ShopImage.GetComponent<Image>().overrideSprite = upgrade1.shopArt;
        option1Name.GetComponent<TextMeshProUGUI>().text = upgrade1.upgradeName;
        option1Description.GetComponent<TextMeshProUGUI>().text = upgrade1.description;

        option2ShopImage.GetComponent<Image>().overrideSprite = upgrade2.shopArt;
        option2Name.GetComponent<TextMeshProUGUI>().text = upgrade2.upgradeName;
        option2Description.GetComponent<TextMeshProUGUI>().text = upgrade2.description;

        option3ShopImage.GetComponent<Image>().overrideSprite = upgrade3.shopArt;
        option3Name.GetComponent<TextMeshProUGUI>().text = upgrade3.upgradeName;
        option3Description.GetComponent<TextMeshProUGUI>().text = upgrade3.description;

        consumableOptionShopImage.GetComponent<Image>().overrideSprite = consumable.shopArt;
        consumableOptionName.GetComponent<TextMeshProUGUI>().text = consumable.upgradeName;
        consumableOptionDescription.GetComponent<TextMeshProUGUI>().text = consumable.description;

        Debug.Log("Shop component activated");
    }

    private void StockShop() {
        Debug.Log("Stocking shop");
        Debug.Log($"Option 1: {option1}");
        Debug.Log($"DropTables: {dropTables}");
        upgrade1 = dropTables.getRandomUpgrade(dropTables.getRandomUpgradeType());
        upgrade2 = dropTables.getRandomUpgrade(dropTables.getRandomUpgradeType());
        upgrade3 = dropTables.getRandomUpgrade(dropTables.getRandomUpgradeType());
        consumable = dropTables.getRandomUpgrade(UpgradeType.Consumable);
        shopIsStocked = true;
        Debug.Log($"Stocked with:  {upgrade1.upgradeName}, {upgrade2.upgradeName}, {upgrade3.upgradeName}, {consumable.upgradeName}");
    }

    public void BuyOption(int i)
    {

        switch (i) {
            case 1:
                Debug.Log($"Buying: {upgrade1.upgradeName}");
                option1.SetActive(false);
                dropTables.purchase(upgrade1);
                HandleUpgrade(upgrade1);
                break;
            case 2:
                Debug.Log($"Buying: {upgrade2.upgradeName}");
                option2.SetActive(false);
                dropTables.purchase(upgrade2);
                HandleUpgrade(upgrade2);
                break;
            case 3:
                Debug.Log($"Buying: {upgrade3.upgradeName}");
                option3.SetActive(false);
                dropTables.purchase(upgrade3);
                HandleUpgrade(upgrade3); ;
                break;
            case 4:
                Debug.Log($"Buying: {consumable.upgradeName}");
                consumableOption.SetActive(false);
                dropTables.purchase(consumable);
                HandleUpgrade(consumable);
                break;
            default: 
                break;
        }

    }

    private void HandleUpgrade(Upgrade upgrade)
    {

    }
}
