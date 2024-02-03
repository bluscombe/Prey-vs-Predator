using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform fishTransform;
    public float smoothSpeed = 0.125f;
    public float zoomLevel = 0.5f; // Zoomed-in value
    public Vector3 offset;

    // Define the boundaries
    public float minX, maxX, minY, maxY;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        // Set the camera's orthographic size for zooming
        cam.orthographicSize = zoomLevel;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(fishTransform.position.x, fishTransform.position.y, transform.position.z) + offset;

        // Clamp the desired position within the specified boundaries
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
