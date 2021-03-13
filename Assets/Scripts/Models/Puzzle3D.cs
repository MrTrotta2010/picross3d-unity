public class Puzzle3D
{
    private SolutionMatrix solutionMatrix; // Matriz de solução do puzzle
    private PuzzleDimensions dimensions; // Dimensões em camadas, linhas e colunas

    public Puzzle3D(PuzzleDimensions dimensions)
    {
        this.dimensions = dimensions;
        solutionMatrix = new SolutionMatrix(dimensions);
    }

    public Puzzle3D(int layers, int lines, int columns)
    {
        dimensions = new PuzzleDimensions(layers, lines, columns);
        solutionMatrix = new SolutionMatrix(dimensions);
    }

    public Puzzle3D(SolutionMatrix solutionMatrix)
    {
        this.solutionMatrix = solutionMatrix;
        this.dimensions = new PuzzleDimensions(solutionMatrix.Lenght, solutionMatrix[0].Length, solutionMatrix[0][0].Length);
    }

    public SolutionMatrix SolutionMatrix { get => solutionMatrix; }
    public PuzzleDimensions Dimensions { get => dimensions; }
}
