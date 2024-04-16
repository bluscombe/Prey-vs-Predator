using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{

    public new AudioSource audio;

    public void loadGameScene()
    {
        SceneManager.LoadScene("Game Scene");
    }

    public void quit()
    {
        Application.Quit();
        Debug.Log("quitting");
    }

    public void playButton()
    {
        audio.Play();
    }
}
