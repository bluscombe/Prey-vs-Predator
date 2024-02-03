using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 5.0f;
    private Vector2 screenBounds;
    private float targetYRotation; // Target rotation around the Y-axis
    public float rotationSpeed = 5.0f; // Speed of rotation

    // Start is called before the first frame update
    void Start()
    {
        // Calculate screen bounds
        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        screenBounds = new Vector2(cameraWidth, cameraHeight);
        targetYRotation = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Get current position
        Vector3 pos = transform.position;

        // Get input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Move the fish based on player input and speed
        pos.x += horizontalInput * speed * Time.deltaTime;
        pos.y += verticalInput * speed * Time.deltaTime;

        // Clamp the position to keep the fish within screen bounds
        pos.x = Mathf.Clamp(pos.x, -screenBounds.x, screenBounds.x);
        pos.y = Mathf.Clamp(pos.y, -screenBounds.y, screenBounds.y);

        // Apply the position
        transform.position = pos;

        // Determine target Y rotation based on input direction
        if (horizontalInput > 0)
        {
            targetYRotation = 180; // Rotate to face right
        }
        else if (horizontalInput < 0)
        {
            targetYRotation = 0; // Rotate to face left
        }

        // Gradually rotate the fish around the Y-axis
        Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, -verticalInput * 45);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
