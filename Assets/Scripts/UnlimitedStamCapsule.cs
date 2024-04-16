using System.Collections;
using UnityEngine;

public class UnlimitedStamCapsule : MonoBehaviour
{
    bool isConsumed = false;
    public GameObject sprite;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isConsumed)
        {
            FishMovement fish = GameObject.FindWithTag("Player").GetComponent<FishMovement>();
            StartCoroutine(ApplyUnlimitedStam(fish, 10)); // Apply unlimited stamina for 10 seconds
            //gameObject.SetActive(false); // Disable the power-up capsule
            isConsumed = true;
            sprite.SetActive(false);
        }
    }

    IEnumerator ApplyUnlimitedStam(FishMovement fish, float duration)
    {
        fish.ApplyUnlimitedPowerUp();

        SetStaminaUse(fish, 0.0f); // Make the player consume 0 stamina

        yield return new WaitForSeconds(duration);

        SetStaminaUse(fish, 20.0f); // Revert the player to full stamina usage
        gameObject.SetActive(false);
    }

    void SetStaminaUse(FishMovement fish, float alpha)
    {
        fish.sprintStaminaUseRate = alpha;
    }
}
