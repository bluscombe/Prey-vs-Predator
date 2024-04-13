using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("Main Menu");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void loadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public static void loadWinScene()
    {
        SceneManager.LoadScene("Win Scene");
    }
}
