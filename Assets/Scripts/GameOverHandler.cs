using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverHandler : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 2f;

    private bool isFading = false;

    public void FadeToBlackAndRestart()
    {
        if (!isFading)
        {
            StartCoroutine(FadeToBlackCoroutine());
        }
    }

    private IEnumerator FadeToBlackCoroutine()
    {
        isFading = true;
        float timer = 0;

        while (timer <= fadeDuration)
        {
            // Increase the alpha of the fade image over time
            fadeImage.color = new Color(0, 0, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure the fade image is fully opaque
        fadeImage.color = Color.black;

        // Restart the game (load the current scene again)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
