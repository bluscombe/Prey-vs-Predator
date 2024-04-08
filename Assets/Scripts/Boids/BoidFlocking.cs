using UnityEngine;
using System.Collections;

public class BoidFlocking : MonoBehaviour
{
    private GameObject Controller;
    private bool inited = false;
    private float minVelocity;
    private float maxVelocity;
    private float randomness;
    private GameObject chasee;

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
                GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity + (Vector2)Calc() * Time.deltaTime;

                // enforce minimum and maximum speeds for the boids
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

    private Vector3 Calc()
    {
        if (Controller == null)
        {
            Debug.LogError("Controller is not set for " + gameObject.name);
            return Vector3.zero;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is not attached to " + gameObject.name);
            return Vector3.zero;
        }

        BoidController boidController = Controller.GetComponent<BoidController>();
        if (boidController == null)
        {
            Debug.LogError("BoidController component not found on controller object.");
            return Vector3.zero;
        }

        Vector3 randomize = new Vector3((Random.value * 2) - 1, (Random.value * 2) - 1, (Random.value * 2) - 1);
        randomize.Normalize();

        // Convert flockVelocity and the boid's velocity to Vector3 before subtraction
        Vector3 flockCenter = boidController.flockCenter - transform.position;
        Vector3 boidVelocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);
        Vector3 flockVelocity = boidController.flockVelocity - boidVelocity;
        Vector3 follow = chasee.transform.position - transform.position;

        return (flockCenter + flockVelocity + follow * 2 + randomize * randomness);
    }



    public void SetController(GameObject theController)
    {
        Controller = theController;
        BoidController boidController = Controller.GetComponent<BoidController>();
        minVelocity = boidController.minVelocity;
        maxVelocity = boidController.maxVelocity;
        randomness = boidController.randomness;
        chasee = boidController.chasee;
        inited = true;
    }
}