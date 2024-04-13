using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverHandler : MonoBehaviour
{
    public GameObject player;
    public Image fadeImage;
    public Text timerText; // Reference to the UI Text for the timer
    public Text objCollectedText;
    public float fadeDuration = 2f;
    private bool isFading = false;
    private float startTime;
    private bool gameIsOver = false;
    //public GameObject winScreen; // Assign in the Inspector
    public static int objectivesCollected = 0; // Tracks how many objectives the player has collected
    public int totalObjectives = 5; // Set this to the total number of objectives in your level
    public GameObject objective;
    public int spawnRate = 3;
    public int spawnTime = 15;
    private GameObject[] otherobjs;

    void Start()
    {
        // Initialize the timer and hide the win screen at start
        startTime = Time.time;
        //winScreen.SetActive(false);

        InvokeRepeating("SpawnObj", 0, spawnTime);
    }

    void Update()
    {
        if (!gameIsOver)
        {
            objCollectedText.text = $"{objectivesCollected}/{totalObjectives}";
            UpdateTimerUI();
            CheckForWinCondition();
        }
    }

    private void SpawnObj()
    {
        if(player.GetComponent<Transform>().position.x <= 0 && player.GetComponent<Transform>().position.x > -30){
            for(int i = 0; i < spawnRate; i++){
                GameObject newObjective = Instantiate(objective, new Vector3(Random.Range(20f, 65f), Random.Range(-11f, 11f), -3), Quaternion.identity);
            }
        }
        else if(player.GetComponent<Transform>().position.x > 0 && player.GetComponent<Transform>().position.x < 30){
            for(int i = 0; i < spawnRate; i++){
                GameObject newObjective = Instantiate(objective, new Vector3(Random.Range(-65f, -20f), Random.Range(-11f, 11f), -3), Quaternion.identity);
             }
        }
        else{
            for(int i = 0; i < spawnRate; i++){
                GameObject newObjective = Instantiate(objective, new Vector3(Random.Range(-20f, 20f), Random.Range(-11f, 11f), -3), Quaternion.identity);
            }
        }
        otherobjs = GameObject.FindGameObjectsWithTag("Objective");
        foreach (GameObject objective in otherobjs)
        {
            if (Mathf.Abs(objective.GetComponent<Transform>().position.x - this.transform.position.x) < 2.0f
               && Mathf.Abs(objective.GetComponent<Transform>().position.y - this.transform.position.y) < 2.0f) {
               SpawnObj();
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
        gameIsOver = true;
        Debug.Log("gg");
        SceneManager.LoadScene("Win Scene");
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
