using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    public void DrawDown()
    {
        GridMovement.isTouchEnabled = false;
        if (Game.DrawEnable)
        {
            Game.DrawEnable = false;
        }
        else
        {
            Game.DrawEnable = true;
        }
    }
    
    public void DeleteDown()
    {
        Game.DrawEnable = false;
    }

    public void Play()
    {
        if (Game.simulationEnabled)
        {
            Game.simulationEnabled = false;
        }
        else
        {
            Game.simulationEnabled = true;
        }
    }

    public void Save()
    {
        if (Game.save)
        {
            Game.save = false;
        }
        else
        {
            Game.save = true;
        }
    }

    public void Load()
    {
        if (Game.load)
        {
            Game.load = false;
        }
        else
        {
            Game.load = true;
            Game.DrawEnable = false;
        }
    }

    public void Cross()
    {
        Game game = new Game();
    }
}
