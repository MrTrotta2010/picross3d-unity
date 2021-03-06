using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private InputManager inputManager = null;
    private bool isPartOfSolution = false;
    private bool isPainted = false;

    public delegate void OnWrongCubeBroken(GameObject cube);
    public delegate void OnCorrectCubeBroken();

    public delegate void OnCubePainted(GameObject cube);
    public delegate void OnCubeUnpainted(GameObject cube);

    // Jogador quebrou um cubo que era parte da solução
    public static event OnWrongCubeBroken WrongCubeBroken;
    // Jogador quebrou um cubo que não era parte da solução
    public static event OnCorrectCubeBroken CorrectCubeBroken;
    // Jogador pintou um cubo
    public static event OnCubePainted CubePainted;
    // Jogador "despintou" um cubo
    public static event OnCubeUnpainted CubeUnpainted;

    private void Awake()
    {
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
    }

    private void OnMouseDown()
    {
        if (inputManager.BreakModeIsActive())
        {
            if (!isPainted)
            {
                if (isPartOfSolution)
                {
                    WrongCubeBroken?.Invoke(gameObject);
                }
                else
                {
                    CorrectCubeBroken?.Invoke();
                    Destroy(gameObject);
                }
            }
        }
        else if (inputManager.PaintModeIsActive())
        {
            if (isPainted)
            {
                isPainted = false;
                CubeUnpainted?.Invoke(gameObject);
            }
            else {
                isPainted = true;
                CubePainted?.Invoke(gameObject);
            }
        }
    }

    public void SetPartOfSolution(bool value)
    {
        isPartOfSolution = value;
    }
}
