using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 50f;      // Speed of the projectile
    private float damage;
    public int enemyStunDelayInMilli = 0;

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
            enemy.TakeDamage(damage, enemyStunDelayInMilli);
            Destroy(gameObject); // Destroy the projectile after hitting an enemy
        }
        else
        {
            // Destroy the projectile if it hits any other object
            Destroy(gameObject);
        }
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}
