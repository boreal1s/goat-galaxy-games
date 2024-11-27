using UnityEngine;
using System.Collections;

public class EnvHazard : MonoBehaviour
{
    public float damage = 10f; // Damage dealt to the player
    public float damageCooldown = 1f; // Time between damage instances

    private bool canDamage = true; // Tracks if the hazard can deal damage

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player") && canDamage)
        {
            // Access the PlayerController script on the player
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage); // Deal damage to the player
                StartCoroutine(DamageCooldown()); // Start cooldown
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false; // Disable damage temporarily
        yield return new WaitForSeconds(damageCooldown); // Wait for cooldown
        canDamage = true; // Re-enable damage
    }
}
