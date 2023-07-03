using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class MenuControls : MonoBehaviourPunCallbacks
{
    public void PlayMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PlayGarage()
    {
        SceneManager.LoadScene("GLMP_3");
        Game.SCREEN_WIDTH = 60;
        Game.SCREEN_HEIGHT = 80;
    }

    public void PlayMain()
    {
        Game.SCREEN_WIDTH = 500;
        Game.SCREEN_HEIGHT = 500;
        Game.simulationEnabled = true;
        GridMovement.ToggleTouchMovement();
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}
