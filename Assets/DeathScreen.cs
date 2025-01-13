using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}
