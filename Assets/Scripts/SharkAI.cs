using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkAI : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 1.0f; // Slower rotation for more realistic turning
    public Transform moveSpot;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float approachThreshold = 1f; // Increased threshold for smoother stop

    private float waitTime;
    public float startWaitTime = 3f; // Longer wait time for less frequent changes
    private Vector3 targetMoveSpot;
    private float currentSpeed;

    void Start()
    {
        waitTime = startWaitTime;
        targetMoveSpot = GenerateRandomMoveSpot();
    }

    void Update()
    {
        MoveShark();
        RotateShark();

        if (Vector3.Distance(transform.position, targetMoveSpot) < approachThreshold)
        {
            if (waitTime <= 0)
            {
                targetMoveSpot = GenerateRandomMoveSpot();
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    Vector3 GenerateRandomMoveSpot()
    {
        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), -3f); // Ensure the Z-axis is consistent
    }

    void MoveShark()
    {
        // Calculate the speed based on the distance to the target spot to slow down when approaching
        float distance = Vector3.Distance(transform.position, targetMoveSpot);
        currentSpeed = Mathf.Lerp(0, speed, distance / approachThreshold);

        transform.position = Vector3.MoveTowards(transform.position, targetMoveSpot, currentSpeed * Time.deltaTime);
    }

    void RotateShark()
    {
        Vector3 targetDirection = (targetMoveSpot - transform.position).normalized;
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            targetRotation *= Quaternion.Euler(0, 90, 0); // Adjust based on the shark model's orientation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
