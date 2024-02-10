using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform fishTransform; // Fish to follow
    public float smoothSpeed = 0.125f; // How smoothly the camera follows
    public Vector3 offset; // Offset from the fish

    // Define the bounds for camera movement
    public Vector2 minCameraPos;
    public Vector2 maxCameraPos;

    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(fishTransform.position.x, fishTransform.position.y, transform.position.z) + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Clamp the camera's position to ensure it stays within the predefined bounds
        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minCameraPos.x, maxCameraPos.x);
        smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minCameraPos.y, maxCameraPos.y);

        transform.position = smoothedPosition;
    }
}
