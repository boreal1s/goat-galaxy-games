using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private string interactText;

    private ShopTrigger shopTrigger;

    public void Interact(string colliderName) {
        Debug.Log(colliderName);
        if(colliderName == "Shop Trigger") {
            Debug.Log("Shop opened");
            shopTrigger = FindObjectOfType<ShopTrigger>(); // Find the ShopManager in the scene
            if (shopTrigger != null)
            {
                shopTrigger.OpenShop();
            }
        }
    }

    public string GetInteractText() {
        return interactText;
    }
}
