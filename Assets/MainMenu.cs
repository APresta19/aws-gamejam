using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void Options(GameObject optionsMenu)
    {
        optionsMenu.SetActive(true);
        gameObject.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Back(GameObject optionsMenu)
    {
        optionsMenu.SetActive(false);
        gameObject.SetActive(true);
    }
    public void PlayAnimation(Animator anim)
    {
        anim.SetTrigger("Highlighted");
    }
    
}
