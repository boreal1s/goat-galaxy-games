using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f;

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

        // Destroy the projectile after 5 seconds
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projectile collided with: " + other.gameObject.name);

        // Prevent self-collision
        if (other.gameObject == gameObject)
            return;

        // Check if the projectile hit an enemy
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Destroy the projectile after hitting an enemy
        }
        else if (!other.CompareTag("Player"))
        {
            // Destroy the projectile if it hits anything else except the player
            Destroy(gameObject);
        }
    }
}
