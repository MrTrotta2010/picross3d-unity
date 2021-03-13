using UnityEngine;

public class RandomPuzzleGenerator : MonoBehaviour
{
    // Maior dimens�o poss�vel
    [SerializeField] private int maxDimension = 8;

    // As dimens�es do puzzle a ser gerado
    private int layers = 3;
    private int lines = 3;
    private int columns = 3;

    // Refer�ncia a um PuzzleManager
    [SerializeField] PuzzleManager puzzleManager = null;

    // A matriz de solu��o do puzzle que ser� gerada aleatoriamente
    private SolutionMatrix solutionMatrix;

    // Gerador de n�mero aleat�rios
    private System.Random rand;

    // Start is called before the first frame update
    void Start()
    {
        rand = new System.Random();
        GenerateRandomPuzzle();
    }

    public void GenerateRandomPuzzle()
    {
        // Decide aleatoriamente se as dimens�es do puzzle mudar�o
        if (rand.NextDouble() >= 0.5)
            layers = rand.Next(2, maxDimension + 1);
        if (rand.NextDouble() >= 0.5)
            lines = rand.Next(2, maxDimension + 1);
        if (rand.NextDouble() >= 0.5)
            columns = rand.Next(2, maxDimension + 1);

        solutionMatrix = new SolutionMatrix(layers, lines, columns);

        for (int k = 0; k < layers; k++)
        {
            for (int i = 0; i < lines; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Decide aleatoriamente se aquele bloco faz parte da solu��o
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
