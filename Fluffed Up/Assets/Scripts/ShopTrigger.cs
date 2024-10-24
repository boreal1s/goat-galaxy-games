using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{

    [SerializeField]
    private GameObject shopUI;
    public bool shopIsOpen { get; private set; }


    // Start is called before the first frame update
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
           OpenShop();
        }
    }

     void Start()
    {
        shopUI.SetActive(false);
    }

    public void OpenShop()
    {
        shopUI.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        shopIsOpen = true;
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        shopIsOpen = false;
    }
}
