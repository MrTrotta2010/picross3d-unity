using UnityEngine;

public class Block : MonoBehaviour
{
    private InputManager inputManager = null;
    private bool isPartOfSolution = false;
    private bool isPainted = false;

    public delegate void OnWrongBlockBroken(GameObject cube);
    public delegate void OnCorrectBlockBroken();

    public delegate void OnBlockPainted(GameObject cube);
    public delegate void OnBlockUnpainted(GameObject cube);

    // Jogador quebrou um bloco que era parte da solução
    public static event OnWrongBlockBroken WrongBlockBroken;
    // Jogador quebrou um bloco que não era parte da solução
    public static event OnCorrectBlockBroken CorrectBlockBroken;
    // Jogador pintou um bloco
    public static event OnBlockPainted BlockPainted;
    // Jogador "despintou" um bloco
    public static event OnBlockUnpainted BlockUnpainted;

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
                    WrongBlockBroken?.Invoke(gameObject);
                }
                else
                {
                    CorrectBlockBroken?.Invoke();
                    Destroy(gameObject);
                }
            }
        }
        else if (inputManager.PaintModeIsActive())
        {
            if (isPainted)
            {
                isPainted = false;
                BlockUnpainted?.Invoke(gameObject);
            }
            else {
                isPainted = true;
                BlockPainted?.Invoke(gameObject);
            }
        }
    }

    public void SetPartOfSolution(bool value)
    {
        isPartOfSolution = value;
    }
}
