using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void loadGameScene()
    {
        SceneManager.LoadScene("Game Scene");
    }

    public void quit()
    {
        Debug.Log("quitting");
    }
}
