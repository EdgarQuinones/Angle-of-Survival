using UnityEngine;
using System.Collections.Generic; // Required for Lists

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int maxBounces = 3;

    private int currentBounces = 0;
    private Rigidbody2D rb;

    // The "Memory" of the bullet
    private List<GameObject> enemiesHit = new List<GameObject>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Ignore Player, Terrain, and enemies we ALREADY hit
        if (collision.CompareTag("Player") || collision.CompareTag("Terrain")) return;
        if (enemiesHit.Contains(collision.gameObject)) return; // Prevent double-hits

        // 2. Add this new enemy to our history so we don't target them again
        if (collision.CompareTag("Enemy"))
        {
            enemiesHit.Add(collision.gameObject);
        }

        // 3. Deal Damage
        if (collision.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
        {
            enemyHealth.TakeDamage();
        }

        // 4. Check for Ricochet
        if (currentBounces < maxBounces)
        {
            Ricochet(collision.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Ricochet(GameObject currentTarget)
    {
        float detectionRadius = 10f; // Increased radius to find further targets
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D hit in potentialTargets)
        {
            // Skip non-enemies
            if (!hit.CompareTag("Enemy")) continue;

            // THE FIX: Skip anyone we have EVER hit (including the one we just hit)
            if (enemiesHit.Contains(hit.gameObject)) continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = hit.transform;
            }
        }

        if (bestTarget != null)
        {
            currentBounces++;

            Vector2 direction = (bestTarget.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        else
        {
            // If no NEW enemies are found, destroy the bullet
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}