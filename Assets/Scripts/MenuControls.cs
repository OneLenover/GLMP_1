using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour
{
    public void PlayMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PlayGarage()
    {
        SceneManager.LoadScene("GLMP_3");
    }

    public void PlayMain()
    {
        SceneManager.LoadScene("MainField");
        Game.SCREEN_WIDTH = 500;
        Game.SCREEN_HEIGHT = 500;
        GridMovement.ToggleTouchMovement();
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}
