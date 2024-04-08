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

    public BoidController boidController;

    public Slider healthBar;
    public Slider staminaBar;
    public Slider invisibilityTimerSlider;
    public float maxHealth = 100f;
    public float health;
    public float maxStamina = 100f;
    public float stamina;
    public float staminaRecoveryRate = 5f; // Stamina recovered per second
    public float sprintStaminaUseRate = 20f; // Stamina used per second when moving

    public Camera playerCamera; // Assign this in the Inspector
    public float zoomOutFOV = 60f; // New FOV value for zoomed-out effect
    private float originalFOV;

    public bool isInvisible = false;

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

        //// If not assigned, try to find the BoidController in the scene
        //if (boidController == null)
        //{
        //    boidController = FindObjectOfType<BoidController>();
        //}

        //if (boidController == null)
        //{
        //    Debug.LogError("BoidController not assigned on " + gameObject.name);
        //    this.enabled = false; // Disable script to prevent further errors
        //    return;
        //}

        //if (boidController.chasee == null)
        //{
        //    Debug.LogError("Chasee not assigned on BoidController for " + gameObject.name);
        //    this.enabled = false; // Disable script to prevent further errors
        //    return;
        //}
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

        //// Apply the fish's current position to the chasee object
        //if (boidController != null && boidController.chasee != null)
        //{
        //    boidController.chasee.transform.position = this.transform.position;
        //}

        //healthBar.value = health;
        //staminaBar.value = stamina;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ObjectiveItem"))
        {
            ApplyInvisibilityPowerUp();
            Destroy(other.gameObject);
        }
    }

    void ApplyInvisibilityPowerUp()
    {
        StartCoroutine(BecomeInvisible(10));
    }

    IEnumerator BecomeInvisible(float duration)
    {
        float endTime = Time.time + duration;
        isInvisible = true; // Set isInvisible to true to indicate the fish is now invisible
        SetTransparency(0.7f);

        invisibilityTimerSlider.gameObject.SetActive(true); // Activate the slider when invisibility starts
        invisibilityTimerSlider.maxValue = duration;
        invisibilityTimerSlider.value = duration; // Initialize the slider value to full duration

        // Countdown the duration of the invisibility
        while (Time.time < endTime && isInvisible)
        {
            invisibilityTimerSlider.value = endTime - Time.time; // Update the slider value to represent the remaining time
            yield return null;
        }

        isInvisible = false; // Reset isInvisible to false after the duration ends
        SetTransparency(1.0f);
        invisibilityTimerSlider.gameObject.SetActive(false); // Disable the slider when invisibility ends
    }


    void SetTransparency(float alpha)
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            foreach (var material in renderer.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    Color newColor = material.color;
                    newColor.a = alpha;
                    material.color = newColor;
                }
            }
        }
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
