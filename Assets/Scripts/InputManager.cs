using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool breakModeActive = false;
    private bool paintModeActive = false;

    void Update()
    {
        // Modo destrutivo
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
        {
            if (!paintModeActive) breakModeActive = true;
        }
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
        {
            breakModeActive = false;
        }

        // Modo pintura
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            if (!breakModeActive) paintModeActive = true;
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            paintModeActive = false;
        }
    }

    public bool BreakModeIsActive()
    {
        return breakModeActive;
    }

    public bool PaintModeIsActive()
    {
        return paintModeActive;
    }
}
