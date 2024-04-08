using UnityEngine;
using System.Collections;

public class BoidController : MonoBehaviour
{
    public float minVelocity = 5;
    public float maxVelocity = 20;
    public float randomness = 1;
    public int flockSize = 20;
    public GameObject prefab;
    public GameObject chasee;

    public Vector3 flockCenter;
    public Vector3 flockVelocity;

    private GameObject[] boids;

    void Start()
    {
        boids = new GameObject[flockSize];
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogError("BoxCollider2D is not attached to " + gameObject.name);
            return;
        }

        for (var i = 0; i < flockSize; i++)
        {
            Vector3 position = new Vector3(
                Random.value * collider.bounds.size.x,
                Random.value * collider.bounds.size.y,
                0) - collider.bounds.extents;

            position += transform.position; // Ensure the position is relative to the BoidController object

            GameObject boid = Instantiate(prefab, position, Quaternion.identity) as GameObject;
            if (boid == null)
            {
                Debug.LogError("Boid instantiation failed");
                continue;
            }

            boid.transform.parent = transform;
            boid.GetComponent<BoidFlocking>().SetController(gameObject);
            boids[i] = boid;

            Debug.Log("Spawned boid at: " + boid.transform.position);

        }
    }


    void Update()
    {
        Vector3 theCenter = Vector3.zero;
        Vector3 theVelocity = Vector3.zero;

        int realFlockSize = 0; // Keep track of non-null boids

        foreach (GameObject boid in boids)
        {
            // Check if boid is not null before accessing its properties
            if (boid != null)
            {
                theCenter += boid.transform.position;
                Rigidbody2D rb = boid.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    theVelocity += (Vector3)rb.velocity;
                    realFlockSize++; // Increment for each non-null boid
                }
            }
        }

        // Avoid division by zero if realFlockSize is zero
        if (realFlockSize > 0)
        {
            flockCenter = theCenter / realFlockSize;
            flockVelocity = theVelocity / realFlockSize;
        }
    }

}