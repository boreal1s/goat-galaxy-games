using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 300f; // Speed of the projectile
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
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collision with the player
        if (other.CompareTag("Player"))
            return;

        Destroy(gameObject); // Destroy the projectile after colliding with anything
    }

    public float GetSpeed()
    {
        return speed;
    }
}
