using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private string interactText;

    public  ShopController shopController;

    public void Interact(string colliderName) {
        Debug.Log(colliderName);
        if(colliderName == "ShopTrigger") {
            Debug.Log("Shop opened");
            shopController.OpenShop();
        }
    }

    public string GetInteractText() {
        return interactText;
    }
}
