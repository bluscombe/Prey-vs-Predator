using System.Collections;
using UnityEngine;

public class SchoolOfFish : MonoBehaviour
{
    public GameObject fishPrefab;
    public int numberOfFish = 10;
    public float gridSpacing = 2.0f; // Spacing between each fish at spawn
    public float moveSpeed = 2.0f;
    public Vector2 moveBounds = new Vector2(32, 7); // Adjusted for specific bounds

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
            Vector2 spawnPosition = new Vector2(xPosition, yPosition);

            var fish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(MoveFishToRandomPosition(fish));
        }
    }

    IEnumerator MoveFishToRandomPosition(GameObject fish)
    {
        while (true)
        {
            Vector2 startPosition = fish.transform.position;
            Vector2 randomPosition = new Vector2(
                Random.Range(-moveBounds.x, moveBounds.x),
                Random.Range(-2, 5)
            );

            // Determine if moving right or left by comparing the new position with the current position
            float direction = randomPosition.x - startPosition.x;

            // Flip the fish sprite based on direction
            if (direction > 0)
            {
                fish.transform.localScale = new Vector3(-Mathf.Abs(fish.transform.localScale.x), fish.transform.localScale.y, fish.transform.localScale.z);
            }
            else if (direction < 0)
            {
                fish.transform.localScale = new Vector3(Mathf.Abs(fish.transform.localScale.x), fish.transform.localScale.y, fish.transform.localScale.z);
            }

            while (Vector2.Distance(fish.transform.position, randomPosition) > 0.1f)
            {
                fish.transform.position = Vector2.MoveTowards(fish.transform.position, randomPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(1, 5)); // Wait for a random time before choosing a new destination
        }
    }

}
