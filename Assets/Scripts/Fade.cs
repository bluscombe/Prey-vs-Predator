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

    public void fadeOutToGame(){
        SceneManager.LoadScene("Game Scene");
    }

    public void fadeOutToWin(){
        SceneManager.LoadScene("Win Scene");
    }

    public void winOut(){
        animator.SetTrigger("WinOut");
    }
}
