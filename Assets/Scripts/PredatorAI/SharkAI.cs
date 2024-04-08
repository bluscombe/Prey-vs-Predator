using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SharkAI : MonoBehaviour
{
    public enum SharkState
    {
        Patrol,
        Chase,
        Facing,
        PreparingToCharge,
        Charge,
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

    private Animator sharkAnimator;

    [Header("Sight")]
    public float fieldOfViewAngle = 110f; // Angle for the field of view

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

    [Header("Charge Preparation")]
    public float facingDuration = 0.5f; // How long to face the player before preparing to charge
    public float prepareToChargeDuration = 1f; // How long to pause before actually charging
    private float facingTimer = 0; // Timer for the facing state
    private float prepareToChargeTimer = 0; // Timer for the preparing to charge state

    [Header("Charge")]
    public float chargeSpeed = 10f; // Speed of the shark when charging
    public float chargeCooldown = 5f; // Time before shark can charge again
    public float chargeDuration = 1f; // How long the shark charges
    private float chargeTimer = 0; // Timer to track charge duration
    private float nextChargeTime = 0; // When the shark is allowed to charge again

    private void Start()
    {
        waitTime = startWaitTime;
        targetMoveSpot = GetRandomPosition();
        damageEffect.color = new Color(1, 0, 0, 0); // Ensure the damage effect is invisible at start
        attackTimer = 0;

        // Initialize the Animator component
        sharkAnimator = GetComponent<Animator>();
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
            case SharkState.Facing:
                FacePlayer();
                break;
            case SharkState.PreparingToCharge:
                PrepareToCharge();
                break;
            case SharkState.Charge:
                ChargePlayer();
                break;
            case SharkState.Attack:
                AttackPlayer();
                break;
        }

        sharkAnimator.SetBool("IsSwimming", currentState != SharkState.Attack);

        CheckTransitions();

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Transition conditions
        if (currentState == SharkState.Patrol && IsPlayerInFieldOfView())
        {
            currentState = SharkState.Chase;
        }
        else if (currentState == SharkState.Chase && !IsPlayerInFieldOfView())
        {
            // Lost sight of the player; consider going back to Patrol or implementing a "search" behavior
            currentState = SharkState.Patrol;
        }
    }

    bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);

        if (angle < fieldOfViewAngle * 0.5f && directionToPlayer.magnitude < detectionRadius)
        {
            // Perform an additional raycast check to ensure there are no obstacles blocking the view
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRadius))
            {
                // Check if the hit object is the player
                if (hit.transform == player)
                {
                    return true; // Player is within field of view and no obstacles are blocking the view
                }
            }
        }
        return false;
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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRadius && distanceToPlayer < detectionRadius && Time.time >= nextChargeTime)
        {
            currentState = SharkState.Facing;
        }
        else
        {
            MoveTo(player.position); // Continue chasing normally if not ready to face/charge
        }
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
            biteAnimationUI.GetComponent<Animator>().SetTrigger("TriggerBite");
            StartCoroutine(HideBiteAnimationUI(0.7f));

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

    void FacePlayer()
    {
        RotateTowards(player.position); // Ensure shark is facing the player

        if (IsFacingPlayer())
        {
            facingTimer += Time.deltaTime;
            if (facingTimer >= facingDuration)
            {
                // Randomly decide whether to charge or not after facing
                if (Random.value > 0.5f) // 50% chance to proceed with charge
                {
                    currentState = SharkState.PreparingToCharge;
                }
                else
                {
                    currentState = SharkState.Chase; // Resume chasing without charging
                }
                facingTimer = 0; // Reset for next time
            }
        }
    }

    // Utility to check if the shark is facing towards the player within a certain angle threshold
    bool IsFacingPlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, dirToPlayer);
        return dotProduct > 0.95; // Adjust this threshold as necessary
    }

    void PrepareToCharge()
    {
        if (prepareToChargeTimer < prepareToChargeDuration)
        {
            prepareToChargeTimer += Time.deltaTime;
        }
        else
        {
            prepareToChargeTimer = 0;
            // Only transition to Charge state sometimes; other times, just resume chasing
            if (Random.value > 0.3f) // 70% chance to charge
            {
                currentState = SharkState.Charge;
                // Randomize charge speed for unpredictability
                chargeSpeed = Random.Range(8f, 12f); // Example range, adjust as needed
            }
            else
            {
                currentState = SharkState.Chase; // Opt to not charge this time
            }
        }
    }

    void ChargePlayer()
    {
        if (chargeTimer <= chargeDuration)
        {
            MoveTo(player.position, chargeSpeed); // Execute the charge
            chargeTimer += Time.deltaTime;

            // Implement a contact check with the player within the attack radius
            if (Vector3.Distance(transform.position, player.position) <= attackRadius)
            {
                // Implement attack logic here
                currentState = SharkState.Attack; // Switch to attack state
                nextChargeTime = Time.time + chargeCooldown; // Reset the cooldown for the next charge
                chargeTimer = 0; // Reset charge timer
            }
        }
        else
        {
            currentState = SharkState.Chase; // Return to chase if the charge finished
            chargeTimer = 0; // Reset the charge timer for the next charge
        }
    }

    void MoveTo(Vector3 target, float currentSpeed)
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, target.y, transform.position.z), currentSpeed * Time.deltaTime);
        RotateTowards(target);
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