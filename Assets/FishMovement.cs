using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 5.0f;
    private Vector2 screenBounds;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate screen bounds
        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        screenBounds = new Vector2(cameraWidth, cameraHeight);
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

        // Flip and rotate the fish based on the direction
        if (horizontalInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 180, -verticalInput * 45); // Face right, rotate for up/down movement
        }
        else if (horizontalInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, -verticalInput * 45); // Face left, rotate for up/down movement
        }
        else
        {
            // Only vertical movement, rotate based on vertical direction
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, -verticalInput * 45);
        }
    }
}