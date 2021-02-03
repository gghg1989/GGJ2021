using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    ControlSystem controls;
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject GameplayUI;

    void Start()
    {
        //if no controls, create new one
        if (controls == null)
        {
            controls = new ControlSystem();
        }

        // //enable control input
        controls.Gameplay.Enable();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controls.Gameplay.ESC.triggered)
        {
            ESCPressed();
        }
    }

    private void ESCPressed()
    {
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        Debug.Log("Game Resume");

        // Hide pause menu
        pauseMenuUI.SetActive(false);
        // Show Gameplay UI
        GameplayUI.SetActive(true);
        // Enable player control
        controls.Gameplay.Movement.Enable();
        // Resume game running
        Time.timeScale = 1f;

        GameIsPaused = false;
    }

    private void Pause()
    {
        // Show pause menu
        pauseMenuUI.SetActive(true);
        // Hide Gameplay UI
        GameplayUI.SetActive(false);
        // Disable player control
        controls.Gameplay.Movement.Disable();
        // Pause game running
        Time.timeScale = 0f;
        
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
