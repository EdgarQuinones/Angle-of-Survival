using UnityEngine;
using UnityEngine.InputSystem;

public class FaceMouse : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 1. Get Mouse Position from Input System
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // 2. Convert Screen Position to World Position
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        // 3. Calculate Direction (Target - Current)
        Vector2 direction = new Vector2(
            mouseWorldPosition.x - transform.position.x,
            mouseWorldPosition.y - transform.position.y
        );

        // 4. Calculate Angle in Degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. Apply Rotation
        // Subtract 90 if your sprite's "front" faces Up instead of Right
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}