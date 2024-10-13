using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;      // Speed of the projectile
    public float damage = 10f;     // Damage dealt to enemies
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
        else
        {
            Debug.LogError("No Rigidbody attached to the projectile.");
        }

        // Destroy the projectile after 5 seconds to prevent clutter
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collision with the player
        if (other.CompareTag("Player"))
            return;

        // Check if the projectile hit an enemy
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Destroy the projectile after hitting an enemy
        }
        else
        {
            // Destroy the projectile if it hits any other object
            Destroy(gameObject);
        }
    }
}
