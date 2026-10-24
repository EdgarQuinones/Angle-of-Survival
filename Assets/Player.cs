using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // --- Components ---
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    public Health healthScript; // Reference to your new Health script

    // --- Movement Settings ---
    public float speed = 5f;
    private Vector2 _moveDirection;

    // --- Input System ---
    public InputActionReference move;
    public InputActionReference shoot;

    // --- Bullet System ---
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        // Automatically find the Health script on this same object
        healthScript = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (move != null && move.action != null)
            move.action.Enable();

        if (shoot != null && shoot.action != null)
        {
            shoot.action.Enable();
            shoot.action.performed += ctx => Shoot(); // Changed 'started' to 'performed' for cleaner tapping
        }
    }

    private void OnDisable()
    {
        if (move != null && move.action != null)
            move.action.Disable();

        if (shoot != null && shoot.action != null)
            shoot.action.performed -= ctx => Shoot();
    }

    private void Update()
    {
        // 1. CRITICAL FIX: Stop all logic if dead
        if (healthScript.isDead)
        {
            // Optional: Force idle state if you don't have a specific "Dead" state in animator
            // animator.SetBool("isRunning", false); 
            return;
        }

        // 2. Movement Input
        _moveDirection = move.action.ReadValue<Vector2>();

        // 3. Animation State
        bool isRunning = _moveDirection.magnitude > 0;
        animator.SetBool("isRunning", isRunning);

        // 4. Mouse Aiming & Flipping
        HandleRotation();
    }

    private void FixedUpdate()
    {
        // 5. Physics Movement
        if (healthScript.isDead)
        {
            rb.linearVelocity = Vector2.zero; // Stop sliding if we die while moving
            return;
        }

        rb.MovePosition(rb.position + _moveDirection * speed * Time.fixedDeltaTime);
    }

    private void HandleRotation()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        // Calculate Direction to mouse
        float directionX = mouseWorldPosition.x - transform.position.x;

        // Flip sprite based on mouse position
        if (directionX > 0)
        {
            spriteRenderer.flipX = false; // Face Right
        }
        else if (directionX < 0)
        {
            spriteRenderer.flipX = true; // Face Left
        }
    }

    private void Shoot()
    {
        if (healthScript.isDead) return; // Can't shoot if dead

        if (bulletPrefab == null || bulletSpawnPoint == null)
        {
            Debug.LogError("Assign Bullet Prefab and Spawn Point in the Inspector!");
            return;
        }

        animator.SetTrigger("hasShot");

        // Spawn Bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        // Apply Velocity
        if (bullet.TryGetComponent<Rigidbody2D>(out Rigidbody2D bulletRb))
        {
            bulletRb.linearVelocity = bulletSpawnPoint.up * bulletSpeed;
        }
    }
}