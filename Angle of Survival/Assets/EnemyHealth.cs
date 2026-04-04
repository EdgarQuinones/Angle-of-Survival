using UnityEngine;
using System.Collections; // Required for Coroutines

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;
    public bool isDead = false;

    // --- Visual Effects ---
    [Header("Flash Settings")]
    public Color hurtColor = Color.red;
    public float flashDuration = 0.1f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store the original color (usually white)
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // Note: If your bullets are Triggers, this won't fire. 
    // It's better if your Bullet script calls TakeDamage() directly.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (isDead) return; // Don't take damage if already dead

        health -= 1;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            // If still alive, flash red!
            if (spriteRenderer != null)
            {
                StartCoroutine(FlashRoutine());
            }
        }
    }

    private IEnumerator FlashRoutine()
    {
        // 1. Swap to Hurt Color
        spriteRenderer.color = hurtColor;

        // 2. Wait for a split second
        yield return new WaitForSeconds(flashDuration);

        // 3. Swap back to Normal (only if not dead)
        if (!isDead && spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("isDead");

        // Ensure they don't die while stuck as "Red"
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Disable collision so player can walk over the dead body
        GetComponent<Collider2D>().enabled = false;

        // Stop them from moving (if they have a NavMeshAgent or Follow script)
        // GetComponent<FollowPlayer>().enabled = false; 

        Destroy(gameObject, 2f);
    }
}