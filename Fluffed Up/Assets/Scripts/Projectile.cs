using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 300f; // Speed of the projectile
    private float damage;
    public int enemyStunDelayInMilli = 0;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
            rb.useGravity = true;
        }
        else
        {
            Debug.LogError("No Rigidbody attached to the projectile.");
        }

        // Destroy the projectile after 5 seconds to prevent clutter
        Destroy(gameObject, 5f);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    // Ignore collision with the player
    //    if (other.CompareTag("Player"))
    //        return;

    //    // Check if the projectile hit an enemy
    //    EnemyBase enemy = other.GetComponent<EnemyBase>();
    //    if (enemy != null)
    //    {
    //        enemy.TakeDamage(damage, enemyStunDelayInMilli);
    //    }

    //    Destroy(gameObject); // Destroy the projectile after colliding with anything
    //}

    private EnemyBase GetEnemyBase(Collider obj)
    {
        // Check if the object itself has the EnemyBase component
        if (obj.GetComponent<EnemyBase>() != null)
        {
            return obj.GetComponent<EnemyBase>();
        }

        // Traverse up the hierarchy to check its parent objects
        Transform current = obj.transform;
        while (current.parent != null)
        {
            current = current.parent;
            if (current.GetComponent<EnemyBase>() != null)
            {
                return current.GetComponent<EnemyBase>();
            }
        }

        // No EnemyBase component found in the hierarchy
        return null;
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
