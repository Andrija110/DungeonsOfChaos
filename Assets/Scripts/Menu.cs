using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnPlayButton()
    {
        //SceneManager.LoadScene(1);       
        //Game.Instance.GoToScene("MainArea");
        Game.Instance.GoToScene("Name");
    }

    public void OnMenuButton()
    {
        Game.Instance.GoToScene("Menu");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void OnScoreButton()
    {
        Game.Instance.GoToScene("Score");
    }

    public void OnTutorialButton()
    {
        Game.Instance.GoToScene("Tutorial");
    }
}
