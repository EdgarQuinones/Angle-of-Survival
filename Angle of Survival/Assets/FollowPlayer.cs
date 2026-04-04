using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    Vector3 targetPosition;
    Rigidbody2D rb;
    bool isFollowing = true;
    public float speed = 5;
    public float minDistance = 0.1f; // Minimum distance to stop following
    public EnemyHealth EnemyHealthScript; // Reference to the EnemyHealth script

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        EnemyHealthScript = GetComponent<EnemyHealth>(); // Get the EnemyHealth component
    }

    private void Update()
    {
        if (EnemyHealthScript.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        float distance = Vector2.Distance(rb.position, targetPosition);

        if (distance > minDistance)
        {
            isFollowing = true;
        }
    }

    private void FixedUpdate()
    {   
        if (EnemyHealthScript.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isFollowing)
        {
            float distance = Vector2.Distance(rb.position, targetPosition);

            // Move if not at the location (using your size-based stopping distance)
            if (distance > minDistance)
            {
                // Inside FixedUpdate
                Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
            }
            else
            {
                // Fully stop the unit so it doesn't drift
                rb.linearVelocity = Vector2.zero;
                isFollowing = false;
            }
        }
    }
}
