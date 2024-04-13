using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    public void restartGame(){
        SceneManager.LoadScene("Game Scene");
    }
}
