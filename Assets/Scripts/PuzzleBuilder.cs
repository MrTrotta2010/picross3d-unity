using UnityEngine;

public class PuzzleBuilder : MonoBehaviour
{
    // Referencia para um PuzzleManager
    [SerializeField] private PuzzleManager puzzleManager = null;
    // Bool que indica se os blocos que fazem parte da solução serão destacados
    public bool highlightCorrectBlocks = false;

    void Start()
    {
        puzzleManager.BuildPuzzleFromModel(highlightCorrectBlocks);
    }
}
