using UnityEngine;
using System.Collections; // Required for Coroutines

public class Health : MonoBehaviour
{
    public Animator animator;
    public float health = 3;
    public bool isDead = false;

    [Header("Invincibility Settings")]
    public float damageCooldown = 1.0f;
    private float lastHitTime;

    [Header("Visual Effects")]
    public Color hurtColor = Color.red;   // The color to flash when hit
    public float flashDuration = 0.1f;    // How fast the flash is (0.1 is snappy)

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save the normal color (usually white) so we can switch back to it
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Check Cooldown
            if (Time.time > lastHitTime + damageCooldown && !isDead)
            {
                TakeDamage();
                lastHitTime = Time.time;
            }
        }
    }

    public void TakeDamage()
    {
        health -= 1;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Trigger the Flash Effect
            if (spriteRenderer != null)
            {
                StartCoroutine(FlashRoutine());
            }
        }
    }

    private IEnumerator FlashRoutine()
    {
        // 1. Turn Red
        spriteRenderer.color = hurtColor;

        // 2. Wait
        yield return new WaitForSeconds(flashDuration);

        // 3. Turn back to Normal (only if we aren't dead)
        if (!isDead)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);

        // Reset color just in case they died mid-flash
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        Debug.Log("Player Died");
    }
}