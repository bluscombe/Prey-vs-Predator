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
    public RectTransform biteAnimationUI;
    public RectTransform bloodAnimationUI;

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

    [Header("Attack")]
    public float attackCooldown = 2f; // Time in seconds between attacks
    private float attackTimer;

    private void Start()
    {
        waitTime = startWaitTime;
        targetMoveSpot = GetRandomPosition();
        damageEffect.color = new Color(1, 0, 0, 0); // Ensure the damage effect is invisible at start
        attackTimer = 0;
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

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the shark can attack based on cooldown and distance to the player
        if (attackTimer <= 0 && distanceToPlayer <= attackRadius)
        {

            // Convert world position of the shark bite to screen position
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(player.position);

            // Assuming the canvas is using Screen Space - Overlay or Screen Space - Camera
            // Convert screen position to local position in the canvas
            Vector2 localPoint;
            RectTransform canvasRectTransform = biteAnimationUI.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPoint, Camera.main, out localPoint);

            // Set the local position of the biteAnimationUI
            biteAnimationUI.anchoredPosition = localPoint;

            // Activate the bite animation UI and play the animation
            biteAnimationUI.gameObject.SetActive(true);
            biteAnimationUI.GetComponent<Animator>().SetTrigger("PlayBiteAnimation");
            StartCoroutine(HideBiteAnimationUI(0.6f));

            //Blood animation
            StartCoroutine(ShowAndFadeOutBlood());

            //// Perform the attack
            //StartCoroutine(ShowDamageEffect());

            // Slow down the shark for 2 seconds to a specified slower speed
            StartCoroutine(SlowDown(2f, speed * 0.5f)); // Example: slow down to half speed for 2 seconds

            // Assuming the player's FishMovement script is attached to the same GameObject as the player transform.
            FishMovement playerHealth = player.GetComponent<FishMovement>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(20);
            }

            // Reset to chase after attacking
            currentState = SharkState.Chase;

            // Reset the attack timer
            attackTimer = attackCooldown;
        }
        else if (distanceToPlayer > attackRadius)
        {
            // If the shark is too far away after attacking, consider switching back to chase or patrol state
            currentState = SharkState.Chase; // Or Patrol, depending on your game logic
        }
    }


    void CheckTransitions()
    {
        FishMovement playerFishMovement = player.GetComponent<FishMovement>();
        bool playerIsInvisible = playerFishMovement != null && playerFishMovement.isInvisible;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // When the player is invisible, the shark should not transition to chase or attack
        if (playerIsInvisible)
        {
            if (currentState != SharkState.Patrol)
            {
                currentState = SharkState.Patrol;
                targetMoveSpot = GetRandomPosition(); // Ensure shark continues patrolling by setting a new target
            }
            return; // Skip the rest of the transition checks
        }

        // Existing transition logic when player is not invisible
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

            //Orientation
            Quaternion modelCorrectionRotation = Quaternion.Euler(0, 0, 0);
            targetRotation *= modelCorrectionRotation;

            // Apply the corrected rotation smoothly over time
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), transform.position.z);
    }

    IEnumerator HideBiteAnimationUI(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the animation to complete
        biteAnimationUI.gameObject.SetActive(false); // Then hide the UI element
    }

    private IEnumerator SlowDown(float duration, float slowSpeed)
    {
        float originalSpeed = speed; // Store the original speed
        speed = slowSpeed; // Reduce the shark's speed to the specified slow speed

        yield return new WaitForSeconds(duration); // Wait for the duration of the slow down

        speed = originalSpeed; // Restore the original speed
    }

    private IEnumerator ShowAndFadeOutBlood()
    {
        yield return new WaitForSeconds(0.1f);
        // Activate the blood animation UI
        bloodAnimationUI.gameObject.SetActive(true);

        // Fade out the blood
        Image bloodImage = bloodAnimationUI.GetComponent<Image>();
        float duration = 2.0f; // Duration of fade
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / duration);
            bloodImage.color = new Color(bloodImage.color.r, bloodImage.color.g, bloodImage.color.b, alpha);
            yield return null;
        }

        // Optionally deactivate or reset the blood effect for future use
        bloodAnimationUI.gameObject.SetActive(false);
    }


    //IEnumerator ShowDamageEffect()
    //{
    //    damageEffect.gameObject.SetActive(true); // Activate the image object before changing its color
    //    damageEffect.color = new Color(1, 0, 0, 0.5f); // Red with half transparency
    //    yield return new WaitForSeconds(0.5f);
    //    damageEffect.color = new Color(1, 0, 0, 0); // Back to transparent
    //    damageEffect.gameObject.SetActive(false); // Optionally deactivate the image object after the effect
    //}
}