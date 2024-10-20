using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{

    private GameObject shopComponent;
    public bool shopIsOpen;

    private void Start()
    {
    }

    private void Update()
    {
        if (shopComponent.activeSelf)
            Cursor.lockState = CursorLockMode.None;
    }

    public void OpenShop()
    {
        Debug.Log("Shop opened");
        shopComponent.SetActive(true);
        Time.timeScale = 0;
        shopIsOpen = true;
    }

    public void CloseShop()
    {
        shopComponent.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        shopIsOpen = false;
    }
}
