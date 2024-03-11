using System.Collections;
using UnityEngine;

public class SchoolOfFish : MonoBehaviour
{
    public GameObject fishPrefab;
    public int numberOfFish = 10;
    public float gridSpacing = 2.0f;
    public float moveSpeed = 2.0f;
    public float fleeSpeed = 5f;
    public Vector2 moveBounds = new Vector2(32, 7);
    public Transform sharkTransform;
    public float detectionRadius = 10f;

    void Start()
    {
        SpawnFish();
    }

    void SpawnFish()
    {
        Vector2 startPosition = transform.position;
        for (int i = 0; i < numberOfFish; i++)
        {
            float xPosition = startPosition.x + (i % Mathf.Sqrt(numberOfFish)) * gridSpacing;
            float yPosition = startPosition.y + (i / Mathf.Sqrt(numberOfFish)) * gridSpacing;
            Vector3 spawnPosition = new Vector3(xPosition, yPosition, -3);

            GameObject fish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
            fish.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f); // Adjusted for uniform scaling
            StartCoroutine(MoveFishToRandomPosition(fish));
        }
    }

    IEnumerator MoveFishToRandomPosition(GameObject fish)
    {
        Vector3 originalScale = fish.transform.localScale;

        while (true)
        {
            float distanceToShark = Vector3.Distance(fish.transform.position, sharkTransform.position);
            Vector2 targetPosition;

            if (distanceToShark < detectionRadius)
            {
                Vector2 fleeDirection = (fish.transform.position - sharkTransform.position).normalized;
                targetPosition = (Vector2)fish.transform.position + fleeDirection * 5f;
            }
            else
            {
                targetPosition = new Vector2(
                    Random.Range(-moveBounds.x, moveBounds.x),
                    Random.Range(-moveBounds.y, moveBounds.y)
                );
            }

            float direction = targetPosition.x - fish.transform.position.x;

            // Correcting fish flipping logic
            // Flipping should occur based on the direction of movement. 
            // If direction > 0, fish moves to the right, thus scale.x should be positive (assuming original facing left).
            // If direction < 0, fish moves to the left, thus scale.x should be negative.
            fish.transform.localScale = new Vector3(Mathf.Sign(direction) * -(originalScale.x), originalScale.y, originalScale.z);

            float speed = distanceToShark < detectionRadius ? fleeSpeed : moveSpeed;
            while (Vector2.Distance(fish.transform.position, targetPosition) > 0.1f)
            {
                fish.transform.position = Vector2.MoveTowards(fish.transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(1, 5));
        }
    }
}
