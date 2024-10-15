using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemName; // Name or type of the item
    public int amount; // Amount of the item (if applicable)

    private void setItemData()
    {
        if (gameObject.name == "Heal_Up" || gameObject.name == "Heal_Up(Clone)")
        {
            itemName = "Health";
            amount = 100;
        }
        else if (gameObject.name == "Bullet_Up" || gameObject.name == "Bullet_Up(Clone)")
        {
            itemName = "Bullet";
            amount = 1; 
        }
        else if (gameObject.name == "Coin_Up" || gameObject.name == "Coin_Up(Clone)")
        {
            itemName = "Coin";
            amount = 1; 
        }
        else if (gameObject.name == "Exclamation_Up" || gameObject.name == "Exclamation_Up(Clone)")
        {
            itemName = "Exclamation";
            amount = 1; 
        }
        else if (gameObject.name == "Heart_Up" || gameObject.name == "Heart_Up(Clone)")
        {
            itemName = "Heart";
            amount = 1; 
        }
        else if (gameObject.name == "Power_Down" || gameObject.name == "Power_Down(Clone)")
        {
            itemName = "PowerDown";
            amount = 1; 
        }
        else if (gameObject.name == "Power_Up" || gameObject.name == "Power_Up(Clone)")
        {
            itemName = "PowerUp";
            amount = 1; 
        }
        else if (gameObject.name == "PowerUp_Sphere" || gameObject.name == "PowerUp_Sphere(Clone)")
        {
            itemName = "Sphere";
            amount = 1; 
        }
        else if (gameObject.name == "Question_Up" || gameObject.name == "Question_Up(Clone)")
        {
            itemName = "Question";
            amount = 1;
        }
        else if (gameObject.name == "Rocket_Up" || gameObject.name == "Rocket_Up(Clone)")
        {
            itemName = "Rocket";
            amount = 1;
        }
        else if (gameObject.name == "Star_Up" || gameObject.name == "Star_Up(Clone)")
        {
            itemName = "Star";
            amount = 1;
        }
        else
        {
            Debug.Log("Unknown dropped item: " + gameObject.name);
        }
        // Add more conditions for other item types as needed
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                setItemData();
                // Collect the item
                player.CollectItem(this);
                Destroy(gameObject); // Destroy the item after collection
            }
        }
    }
}
