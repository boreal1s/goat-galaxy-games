using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    private float lerpSpeed = 0.05f;

    public Transform character;
    public float healthHeightOffset = 1.5f;

    private void Update()
    {
        // Make smooth lerp for health
        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed);
        }

        if (character != null)
        {
            // Set the health bar's position to be above the enemy
            Vector3 characterPosition = character.position;
            transform.position = new Vector3(characterPosition.x, characterPosition.y + healthHeightOffset, characterPosition.z);
        }
    }

    // Sets the initial health of player to the health slider aka max health.
    public void SetMaxHealth(float health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
        easeHealthSlider.value = health;
    }

    // When health changes, set new health
    public void SetHealth(float health)
    {
        healthSlider.value = health; // Set slider health to character's health
    }

}
