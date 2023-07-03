using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionControl : MonoBehaviour
{
    public GridOverlay gridOverlay;
    public Color[] newSubColors = new Color[] {
        new Color(0f, 0.5f, 0f, 1f),
        new Color(0f, 0f, 0.5f, 1f), 
        new Color(0.5f, 0f, 0f, 1f), 
    };

    public void Green()
    {
        GridOverlay.subColorStatic = newSubColors[0];
    }

    public void Red()
    {
        GridOverlay.subColorStatic = newSubColors[2];
    }

    public void Blue()
    {
        GridOverlay.subColorStatic = newSubColors[1];
    }
}
