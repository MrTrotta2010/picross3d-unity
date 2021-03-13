using System;

public class SolutionMatrix
{
    private int[][][] matrix;

    public SolutionMatrix(PuzzleDimensions puzzleDimensions)
    {
        matrix = new int[puzzleDimensions.layers][][];
        for (int i = 0; i < puzzleDimensions.layers; i++)
        {
            matrix[i] = new int[puzzleDimensions.lines][];
            for (int j = 0; j < puzzleDimensions.lines; j++)
            {
                matrix[i][j] = new int[puzzleDimensions.columns];
                for (int k = 0; k < puzzleDimensions.columns; k++)
                {
                    matrix[i][j][k] = 0;
                }
            }
        }
    }

    public SolutionMatrix(int layers, int lines, int columns)
    {
        matrix = new int[layers][][];
        for (int i = 0; i < layers; i++)
        {
            matrix[i] = new int[lines][];
            for (int j = 0; j < lines; j++)
            {
                matrix[i][j] = new int[columns];
                for (int k = 0; k < columns; k++)
                {
                    matrix[i][j][k] = 0;
                }
            }
        }
    }

    public int Lenght { get => matrix.Length; }

    // Define um indexador para permitir o uso da notação []
    public int[][] this[int i]
    {
        get
        {
            if (i < 0 || i >= matrix.Length)
            {
                throw new IndexOutOfRangeException("Index " + i + " out of range.");
            }
            return matrix[i];
        }
        set
        {
            if (i < 0 || i >= matrix.Length)
            {
                throw new IndexOutOfRangeException("Index " + i + " out of range.");
            }
            matrix[i] = value;
        }
    }
}
