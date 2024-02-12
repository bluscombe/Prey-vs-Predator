using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SharkAI : MonoBehaviour
{
    public enum SharkState
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("General")]
    public SharkState currentState = SharkState.Patrol;
    public Transform player;
    public Image damageEffect;
    public float speed = 5f;
    public float rotationSpeed = 5f;

    [Header("Patrol")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    private Vector3 targetMoveSpot;

    [Header("Detection")]
    public float detectionRadius = 10f;
    public float attackRadius = 2f;
    private float waitTime;
    public float startWaitTime = 2f;

    private void Start()
    {
        waitTime = startWaitTime;
        targetMoveSpot = GetRandomPosition();
        damageEffect.color = new Color(1, 0, 0, 0); // Ensure the damage effect is invisible at start
    }

    private void Update()
    {
        switch (currentState)
        {
            case SharkState.Patrol:
                Patrol();
                break;
            case SharkState.Chase:
                ChasePlayer();
                break;
            case SharkState.Attack:
                AttackPlayer();
                break;
        }

        CheckTransitions();
    }

    void Patrol()
    {
        MoveTo(targetMoveSpot);

        if (Vector3.Distance(transform.position, targetMoveSpot) < 0.2f)
        {
            if (waitTime <= 0)
            {
                targetMoveSpot = GetRandomPosition();
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    void ChasePlayer()
    {
        MoveTo(player.position);
    }

    void AttackPlayer()
    {
        // Perform the attack
        StartCoroutine(ShowDamageEffect());

        // Reset to chase after attacking
        currentState = SharkState.Chase;
    }

    void CheckTransitions()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (currentState == SharkState.Patrol && distanceToPlayer < detectionRadius)
        {
            currentState = SharkState.Chase;
        }
        else if (currentState == SharkState.Chase)
        {
            if (distanceToPlayer < attackRadius)
            {
                currentState = SharkState.Attack;
            }
            else if (distanceToPlayer > detectionRadius)
            {
                currentState = SharkState.Patrol;
                targetMoveSpot = GetRandomPosition();
            }
        }
    }

    void MoveTo(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, target.y, transform.position.z), speed * Time.deltaTime);
        RotateTowards(target);
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 targetDirection = (target - transform.position).normalized;
        if (targetDirection != Vector3.zero)
        {
            // Determine the target rotation based on the target direction
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            // Adjust the rotation to align the shark's face with the target direction
            // This assumes the shark model needs to be rotated 90 degrees around the Y axis to face forward correctly
            // Adjust this value as necessary to match your model's orientation
            Quaternion modelCorrectionRotation = Quaternion.Euler(0, 90, 0);
            targetRotation *= modelCorrectionRotation;

            // Apply the corrected rotation smoothly over time
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), transform.position.z);
    }

    IEnumerator ShowDamageEffect()
    {
        damageEffect.gameObject.SetActive(true); // Activate the image object before changing its color
        damageEffect.color = new Color(1, 0, 0, 0.5f); // Red with half transparency
        yield return new WaitForSeconds(0.5f);
        damageEffect.color = new Color(1, 0, 0, 0); // Back to transparent
        damageEffect.gameObject.SetActive(false); // Optionally deactivate the image object after the effect
    }
}
