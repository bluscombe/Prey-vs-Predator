using System.Collections;
using UnityEngine;

public class PowerUpCapsule : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ApplyInvisibility(other.gameObject, 10)); // Apply invisibility for 10 seconds
            gameObject.SetActive(false); // Disable the power-up capsule
        }
    }

    IEnumerator ApplyInvisibility(GameObject player, float duration)
    {
        SetTransparency(player, 0.0f); // Make the player fully invisible

        yield return new WaitForSeconds(duration);

        SetTransparency(player, 1.0f); // Revert the player to fully visible
    }

    void SetTransparency(GameObject player, float alpha)
    {
        foreach (var renderer in player.GetComponentsInChildren<Renderer>())
        {
            foreach (var material in renderer.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    Color color = material.color;
                    color.a = alpha;
                    material.color = color;
                }
            }
        }
    }
}
