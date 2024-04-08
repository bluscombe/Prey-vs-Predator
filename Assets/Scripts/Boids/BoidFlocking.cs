using UnityEngine;
using System.Collections;

public class BoidFlocking : MonoBehaviour
{
    private GameObject controller;
    private BoidController boidController;
    private bool inited = false;
    private float minVelocity;
    private float maxVelocity;
    private float randomness;
    private GameObject chasee;

    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1f;
    public float followWeight = 2f;
    public float separationDistance = 1f; // Distance to maintain from other boids

    void Start()
    {
        StartCoroutine("BoidSteering");
    }

    IEnumerator BoidSteering()
    {
        while (true)
        {
            if (inited)
            {
                Vector2 steeringForce = Calc() * Time.deltaTime;
                ApplyForce(steeringForce);

                // Enforce minimum and maximum speeds for the boids
                float speed = GetComponent<Rigidbody2D>().velocity.magnitude;
                if (speed > maxVelocity)
                {
                    GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * maxVelocity;
                }
                else if (speed < minVelocity)
                {
                    GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * minVelocity;
                }
            }

            float waitTime = Random.Range(0.3f, 0.5f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private Vector2 Calc()
    {
        if (controller == null)
        {
            Debug.LogError("Controller is not set for " + gameObject.name);
            return Vector2.zero;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is not attached to " + gameObject.name);
            return Vector2.zero;
        }

        BoidController boidController = controller.GetComponent<BoidController>();
        if (boidController == null)
        {
            Debug.LogError("BoidController component not found on controller object.");
            return Vector2.zero;
        }

        // Get the steering vectors from the different behaviors
        Vector2 alignment = AlignWithBoids() * alignmentWeight;
        Vector2 cohesion = SteerTowards(boidController.flockCenter) * cohesionWeight;
        Vector2 separation = SeparateFromBoids() * separationWeight;
        Vector2 follow = SteerTowards(chasee.transform.position) * followWeight;

        // Calculate the combined force
        Vector2 combinedForce = alignment + cohesion + separation + follow;

        // Add some randomness
        combinedForce += new Vector2(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness));

        return combinedForce;
    }

    private Vector2 SteerTowards(Vector2 target)
    {
        Vector2 desiredDirection = (target - (Vector2)transform.position).normalized;
        return desiredDirection * maxVelocity - GetComponent<Rigidbody2D>().velocity;
    }

    private Vector2 AlignWithBoids()
    {
        if (!inited) return Vector2.zero;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return Vector2.zero;

        // Make sure we have a valid boidController reference
        if (boidController == null) return Vector2.zero;

        // Convert flockVelocity to Vector2 before subtraction
        Vector2 alignVector = (Vector2)boidController.flockVelocity - rb.velocity;
        return alignVector.normalized;
    }


    private Vector2 SeparateFromBoids()
    {
        Vector2 separateVector = Vector2.zero;
        var boids = FindObjectsOfType<BoidFlocking>();
        foreach (var otherBoid in boids)
        {
            if (otherBoid != this && (otherBoid.transform.position - transform.position).magnitude < separationDistance)
            {
                separateVector -= (Vector2)(otherBoid.transform.position - transform.position);
            }
        }
        return separateVector.normalized;
    }

    private void ApplyForce(Vector2 force)
    {
        GetComponent<Rigidbody2D>().velocity += force;
    }

    public void SetController(GameObject theController)
    {
        controller = theController;
        boidController = controller.GetComponent<BoidController>();

        if (boidController == null)
        {
            Debug.LogError("Failed to find BoidController on the assigned GameObject.");
            return;
        }

        minVelocity = boidController.minVelocity;
        maxVelocity = boidController.maxVelocity;
        randomness = boidController.randomness;
        chasee = boidController.chasee;
        inited = true;
    }
}
