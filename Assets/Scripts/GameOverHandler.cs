using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverHandler : MonoBehaviour
{
    public Image fadeImage;
    public Text timerText; // Reference to the UI Text for the timer
    public float fadeDuration = 2f;
    private bool isFading = false;
    private float startTime;
    private bool gameIsOver = false;
    //public GameObject winScreen; // Assign in the Inspector
    public static int objectivesCollected = 0; // Tracks how many objectives the player has collected
    public int totalObjectives = 4; // Set this to the total number of objectives in your level
    public GameObject objective;

    void Start()
    {
        // Initialize the timer and hide the win screen at start
        startTime = Time.time;
        //winScreen.SetActive(false);
    }

    void Update()
    {
        if (!gameIsOver)
        {
            UpdateTimerUI();
            CheckForWinCondition();
            if ((Time.time - startTime) % 10 == 0){
                for(int i = 0; i < 30; i++){
                    GameObject newObjective = Instantiate(objective, new Vector3(Random.Range(-65f, 65f), Random.Range(-11f, 11f), -3), Quaternion.identity);
                }
            }
        }
    }

    private void UpdateTimerUI()
    {
        float timeSinceStart = Time.time - startTime;
        string minutes = ((int)timeSinceStart / 60).ToString("00");
        string seconds = (timeSinceStart % 60).ToString("00");
        timerText.text = $"{minutes}:{seconds}";
    }

    private void CheckForWinCondition()
    {
        if (objectivesCollected >= totalObjectives)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        print("gg");
        gameIsOver = true;
        //winScreen.SetActive(true);
        // Optionally, stop the timer or implement additional win game logic here
    }

    public void FadeToBlackAndRestart()
    {
        if (!isFading && !gameIsOver)
        {
            gameIsOver = true;
            StartCoroutine(FadeToBlackCoroutine());
        }
    }

    private IEnumerator FadeToBlackCoroutine()
    {
        isFading = true;
        float timer = 0;

        while (timer <= fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = Color.black;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
