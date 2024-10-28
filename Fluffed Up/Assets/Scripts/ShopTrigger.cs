using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class ShopTrigger : MonoBehaviour
{
    [SerializeField]
    private ShopController shopController;

    private void Start()
    {
        if (shopController == null)
            Debug.LogWarning("ShopController was not attached to ShopTrigger");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && shopController.canTriggerShop)
        {
           shopController.OpenShop();
        }
    }
}
