using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    public Animator animator;

    public void fadeIn(){
        animator.SetTrigger("FadeIn");
    }

    public void fadeOut(){
        animator.SetTrigger("FadeOut");
    }

    public void mainOut(){
        animator.SetTrigger("MainOut");
    }

    public void tutlOut(){
        animator.SetTrigger("TutlOut");
    }

    public void winOut(){
        animator.SetTrigger("WinOut");
    }

    public void loseOut(){
        animator.SetTrigger("LoseOut");
    }

    public void fadeOutToGame(){
        SceneManager.LoadScene("Game Scene");
    }

    public void fadeOutToWin(){
        SceneManager.LoadScene("Win Scene");
    }

    public void fadeOutToMain(){
        SceneManager.LoadScene("Main Menu");
    }

    public void fadeOutToTutorial(){
        SceneManager.LoadScene("Tutorial Scene");
    }

    public void fadeOutToLose(){
        SceneManager.LoadScene("Lose Scene");
    }
}
