using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    [SerializeField] private Health healthScript; // Reference to your new Health script

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    private Vector2 _moveDirection;
    private Vector2 moveInput;

    //[Header("Input System")]
    //public InputActionReference move;
    //public InputActionReference shoot;

    [Header("Bullet System")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        // Automatically find the Health script on this same object
        healthScript = GetComponent<Health>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {

        // Animation State
        bool isRunning = Mathf.Abs(moveInput.x) > 0.01f;
        animator.SetBool("isRunning", isRunning);

        // Mouse Aiming & Flipping
        HandleRotation();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            moveInput.x * speed,
            moveInput.y * speed
        );
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

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Debug.Log("Shoot");

        if (healthScript.isDead) return;

        if (bulletPrefab == null || bulletSpawnPoint == null)
        {
            Debug.LogError("Assign Bullet Prefab and Spawn Point in the Inspector!");
            return;
        }

        animator.SetTrigger("hasShot");

        GameObject bullet = Instantiate(
            bulletPrefab,
            bulletSpawnPoint.position,
            bulletSpawnPoint.rotation
        );

        if (bullet.TryGetComponent<Rigidbody2D>(out Rigidbody2D bulletRb))
        {
            bulletRb.linearVelocity = bulletSpawnPoint.up * bulletSpeed;
        }
    }
}