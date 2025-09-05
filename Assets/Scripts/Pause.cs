using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Pause : MonoBehaviour
{
    public static bool isGamePaused = false;

    [SerializeField] GameObject pauseMenu;

    private void Start()
    {
        //pauseMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
            isGamePaused = !isGamePaused;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ReturnButton()
    {
        Time.timeScale = 1;
    }

    public void QuitButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        Game.Instance.RestartLevel();
    }
}