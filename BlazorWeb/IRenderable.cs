namespace BlazorWeb;

public interface IRenderable
{
    int X { get; }
    int Y { get; }
    int Width { get; }
    int Height { get; }
    bool Hidden { get; }
}
