using UnityEngine;

public class RandomPuzzleGenerator : MonoBehaviour
{
    [Header("Puzzle dimensions")]
    // As dimensões do puzzle a ser gerado
    [SerializeField] private int layers = 0;
    [SerializeField] private int lines = 0;
    [SerializeField] private int columns = 0;

    [Header("Puzzle manager")]
    // Referência a um PuzzleManager
    [SerializeField] PuzzleManager puzzleManager = null;

    // A matriz de solução do puzzle que será gerada aleatoriamente
    private SolutionMatrix solutionMatrix;

    // Gerador de número aleatórios
    private System.Random rand;

    // Start is called before the first frame update
    void Start()
    {
        rand = new System.Random();
        GenerateRandomPuzzle();
    }

    public void GenerateRandomPuzzle()
    {
        solutionMatrix = new SolutionMatrix(layers, lines, columns);

        for (int k = 0; k < layers; k++)
        {
            for (int i = 0; i < lines; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Decide aleatoriamente se aquele bloco faz parte da solução
                    solutionMatrix[k][i][j] = rand.Next(2);
                }
            }
        }

        if (puzzleManager != null)
            puzzleManager.BuildModelFromPuzzle(new Puzzle3D(solutionMatrix), true);
        else
            Debug.LogError("No Puzzle Manager assigned!");
    }
}
