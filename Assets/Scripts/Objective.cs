using UnityEngine;

public class Objective : MonoBehaviour
{
    // public GameOverHandler gameOverHandler; // Reference to your GameOverHandler script
    private bool isCollected = false;
    private float lerpTime = 1.0f; // Time it takes to lerp to 0
    private Vector3 originalScale; // To store the original scale
    private float currentLerpTime = 0;

    private void Start()
    {
        originalScale = transform.localScale; // Store the original scale
    }

    private void Update()
    {
        if (isCollected)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            float perc = currentLerpTime / lerpTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, perc);

            if (transform.localScale == Vector3.zero)
            {
                Destroy(gameObject); // Destroy the objective once it has shrunk away
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isCollected) // Make sure the collider is tagged with "Player"
        {
            isCollected = true;
            FishMovement.hunger += FishMovement.hungerRecoverRate;
            GameOverHandler.totalSpawned --;
            GameOverHandler.objectivesCollected++; // Notify the GameOverHandler that an objective has been collected
        }
    }
}
