using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour {

    public GameObject pauseMenuUI;
    private String menuScene = "MenuScene";

	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("Escape Button Pressed");
            if(MainScript.timePaused)
            {
                //Debug.Log("Timer is Resumed now");
                Resume();
            }
            else
            {
                //Debug.Log("Timer is Paused now");
                Pause();
            }
        }
	}

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        MainScript.timePaused = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        MainScript.timePaused = false;
        Time.timeScale = 1f;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
