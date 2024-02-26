using System.Collections;
using UnityEngine;

public class PowerUpCapsule : MonoBehaviour
{
    public Camera playerCamera; // Reference to the player's camera
    public float zoomOutFOV = 60f; // Desired FOV for the zoom-out effect
    private float originalFOV; // To store the original FOV

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main; // Automatically find the main camera
        }
        originalFOV = playerCamera.fieldOfView; // Store the original FOV
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { // Make sure the player object has the tag "Player"
            ApplyPowerUp();
            gameObject.SetActive(false); // "Pop" the capsule by disabling it
        }
    }

    void ApplyPowerUp()
    {
        playerCamera.fieldOfView = zoomOutFOV; // Apply the zoom-out effect

        // Optionally, revert the zoom after a delay
        StartCoroutine(RevertZoomAfterDelay(10)); // 10 seconds duration
    }

    IEnumerator RevertZoomAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerCamera.fieldOfView = originalFOV; // Revert FOV to original
    }
}
