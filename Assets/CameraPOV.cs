using UnityEngine;

public class CameraPOV : MonoBehaviour
{
    public Transform fishTransform;
    public float zoomOutFactor = 1.2f; // Adjust this value to zoom out. >1 to zoom out, <1 to zoom in.

    private float verticalLimit; // Calculated based on background size and camera settings
    private float horizontalLimit; // Calculated based on background size and camera settings

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        // Adjust the camera's orthographic size to zoom out a bit
        cam.orthographicSize *= zoomOutFactor;

        float aspectRatio = Screen.width / (float)Screen.height;

        // Adjusted calculations based on the new orthographic size
        horizontalLimit = (6911.8f * 0.7f / 2) - (cam.orthographicSize * aspectRatio);
        verticalLimit = (1091.3f * 0.7f / 2) - cam.orthographicSize;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(fishTransform.position.x, fishTransform.position.y, transform.position.z);

        // Clamp the camera position within the recalculated bounds
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, -horizontalLimit, horizontalLimit);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, -verticalLimit, verticalLimit);

        transform.position = desiredPosition;
    }
}
