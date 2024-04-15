using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private Coroutine popCoroutine; // To keep track of the ongoing pop coroutine

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        if (popCoroutine != null)
        {
            StopCoroutine(popCoroutine); // Stop the current coroutine if it's running
        }
        popCoroutine = StartCoroutine(PopButton());
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        gameObject.SetActive(true);
        Time.timeScale = 1f;
        // Optionally, you might not want to scale the button back up when resuming, but if you do:
        transform.localScale = Vector3.one; // Reset the scale if needed
    }

    public void mainMenu(int sceneID)
    {
        Time.timeScale = 1f;
    }

    private IEnumerator PopButton()
    {
        // Scale down quickly to simulate the start of a pop
        yield return ScaleButton(Vector3.one * 0.7f, 0.1f);
        // Scale up slightly to simulate the bubble expanding before popping
        yield return ScaleButton(Vector3.one, 0.4f);
        gameObject.SetActive(false);
    }

    private IEnumerator ScaleButton(Vector3 targetScale, float duration)
    {
        Vector3 originalScale = transform.localScale;
        float time = 0;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.unscaledDeltaTime; // Use unscaledDeltaTime so it works even when the game is paused
            yield return null;
        }

        transform.localScale = targetScale; // Ensure it ends at the exact target scale
    }
}
