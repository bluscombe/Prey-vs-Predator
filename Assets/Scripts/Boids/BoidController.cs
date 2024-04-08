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
        for (var i = 0; i < flockSize; i++)
        {
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                Debug.LogError("MeshCollider component not found on 'FishV4' GameObject. Please add one.");
                return;
            }

            // Use the bounds of the MeshCollider to find a random position
            Vector3 position = new Vector3(
                Random.value * meshCollider.bounds.size.x,
                Random.value * meshCollider.bounds.size.y,
                Random.value * meshCollider.bounds.size.z
                ) - meshCollider.bounds.extents;

            position = transform.TransformPoint(position); // Transform to world space

            GameObject boid = Instantiate(prefab, position, Quaternion.identity);
            boid.GetComponent<BoidFlocking>().SetController(gameObject);
            boids[i] = boid;
        }
    }


    void Update()
    {
        Vector3 theCenter = Vector3.zero;
        Vector3 theVelocity = Vector3.zero;

        foreach (GameObject boid in boids)
        {
            theCenter = theCenter + boid.transform.localPosition;
            theVelocity = theVelocity + (Vector3)boid.GetComponent<Rigidbody2D>().velocity;
        }

        flockCenter = theCenter / (flockSize);
        flockVelocity = theVelocity / (flockSize);
    }
}