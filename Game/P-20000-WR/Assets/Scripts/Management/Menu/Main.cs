using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject OptionsMenu;

    public void Play()
    {
        // go to next scene (main)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadOptions()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void ExitApplication()
    {
        Debug.Log("Exiting Game... See you next time :)");
        Application.Quit();
    }
}
