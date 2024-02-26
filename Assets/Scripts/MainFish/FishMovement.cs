using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishMovement : MonoBehaviour
{
    public float speed = 1.0f;
    public float sprintSpeed = 2.0f;
    private Vector2 screenBounds;
    private float targetYRotation; // Target rotation around the Y-axis
    public float rotationSpeed = 5.0f; // Speed of rotation

    public Slider healthBar;
    public Slider staminaBar;
    public float maxHealth = 100f;
    public float health;
    public float maxStamina = 100f;
    public float stamina;
    public float staminaRecoveryRate = 5f; // Stamina recovered per second
    public float sprintStaminaUseRate = 20f; // Stamina used per second when moving

    public Camera playerCamera; // Assign this in the Inspector
    public float zoomOutFOV = 60f; // New FOV value for zoomed-out effect
    private float originalFOV;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate screen bounds
        //float cameraHeight = Camera.main.orthographicSize;
        //float cameraWidth = cameraHeight * Camera.main.aspect;
        //screenBounds = new Vector2(cameraWidth, cameraHeight);
        //targetYRotation = transform.eulerAngles.y;
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        // Store the original FOV to revert later
        originalFOV = playerCamera.fieldOfView;

        health = maxHealth;
        stamina = maxStamina;

        healthBar.value = maxHealth;
        staminaBar.value = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        // Get current position
        Vector3 pos = transform.position;

        // Get input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = isSprinting && stamina > 0 ? sprintSpeed : speed; // Use sprint speed if sprinting and stamina is available

        // Move the fish based on player input and speed
        pos.x += horizontalInput * currentSpeed * Time.deltaTime;
        pos.y += verticalInput * currentSpeed * Time.deltaTime;

        //// Clamp the position to keep the fish within screen bounds
        //pos.x = Mathf.Clamp(pos.x, -screenBounds.x, screenBounds.x);
        //pos.y = Mathf.Clamp(pos.y, -screenBounds.y, screenBounds.y);

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

        if (isSprinting && (horizontalInput != 0 || verticalInput != 0))
        {
            UseStamina(Time.deltaTime * sprintStaminaUseRate);
        }
        else
        {
            RecoverStamina(Time.deltaTime * staminaRecoveryRate);
        }

        healthBar.value = health;
        staminaBar.value = stamina;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ObjectiveItem")) // Ensure your objective item has this tag
        {
            CollectObjective(other.gameObject);
        }
    }

    void CollectObjective(GameObject objective)
    {
        // Apply the power-up effect
        ApplyPowerUp();

        // Optionally, destroy the objective item or disable it
        Destroy(objective);
    }

    void ApplyPowerUp()
    {
        // Zoom out by increasing the field of view
        playerCamera.fieldOfView = zoomOutFOV;

        // Optionally, set a timer to revert the zoom after a duration
        StartCoroutine(RevertZoomAfterDelay(10)); // 10 seconds duration
    }

    IEnumerator RevertZoomAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Revert the camera's FOV to its original value
        playerCamera.fieldOfView = originalFOV;
    }

    void UseStamina(float amount)
    {
        stamina -= amount;
        if (stamina < 0)
        {
            stamina = 0;
        }
        staminaBar.value = stamina;
    }

    void RecoverStamina(float amount)
    {
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            stamina += amount;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }

            staminaBar.value = stamina;
        }
    }

    public void TakeDamage(float amount)
    {
        health = health - amount;
        Debug.Log("Damage taken: " + amount + ". Current health: " + health);

        if (health <= 0)
        {
            Die();
            health = 0;
        }
        healthBar.value = health;
    }

    void Die()
    {
        this.enabled = false;
        // Find the GameOverHandler and trigger the fade to black and restart
        GameOverHandler gameOverHandler = FindObjectOfType<GameOverHandler>();
        if (gameOverHandler != null)
        {
            gameOverHandler.FadeToBlackAndRestart();
        }
    }
}
