public class PuzzleDimensions
{
    private int _layers;
    private int _lines;
    private int _columns;

    public PuzzleDimensions(int layers, int lines, int columns)
    {
        _layers = layers;
        _lines = lines;
        _columns = columns;
    }

    public int layers { get => _layers; }
    public int lines { get => _lines; }
    public int columns { get => _columns; }
}
