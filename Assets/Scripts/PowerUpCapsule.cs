using System.Collections;
using UnityEngine;

public class PowerUpCapsule : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FishMovement fish = GameObject.FindWithTag("Player").GetComponent<FishMovement>();
            fish.ApplyInvisibilityPowerUp(); // Apply invisibility for 10 seconds
            //gameObject.SetActive(false); // Disable the power-up capsule
            gameObject.SetActive(false);
        }
    }
}
