using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    private PlayerInputActions playerInputActions;

    private void Start()
    {
        // player input
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.OptionsMenu.performed += OnCallOptionsMenu;

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public void OnCallOptionsMenu(InputAction.CallbackContext context)
    {

        if (!context.performed)
        {
            return;
        }

        ToggleOptionsMenu();
    }

    public void ToggleOptionsMenu()
    {
        if (optionsMenu != null)
        {
            // set the timescale and toggle the menu
            bool isActive = optionsMenu.activeSelf;
            Time.timeScale = isActive ? 1.0f : 0.0f;
            optionsMenu.SetActive(!isActive);
        }
    }

    public void BackToMainMenu()
    {
        // check for timescale
        if (optionsMenu != null && optionsMenu.activeSelf)
        {
            // reset timescale before going back to main
            Time.timeScale = 1.0f;
        }

        // go to prev scene (main menu)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ExitApplication()
    {
        Debug.Log("Exiting Game... See you next time :)");
        Application.Quit();
    }

    void OnSceneUnloaded(Scene current)
    {
        playerInputActions.Player.OptionsMenu.performed -= OnCallOptionsMenu;
    }
}
