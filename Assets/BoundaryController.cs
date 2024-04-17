using UnityEngine;

public class BoundaryController : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Assign the main camera, ensure the main camera tag is set correctly
    }

    void Update()
    {
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(transform.position); // Convert world position to viewport
        float padding = 0.05f; // 5% padding from each edge
        screenPosition.x = Mathf.Clamp(screenPosition.x, padding, 1 - padding);
        screenPosition.y = Mathf.Clamp(screenPosition.y, padding, 1 - padding);
        transform.position = mainCamera.ViewportToWorldPoint(screenPosition); // Convert the clamped viewport position back to world space
    }
}
