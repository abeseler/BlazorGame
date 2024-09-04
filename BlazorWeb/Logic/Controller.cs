using System.Diagnostics.CodeAnalysis;

namespace BlazorWeb.Logic;

public static class Controller
{
    public static List<Control> ActiveControls { get; } = [];

    public static bool TryParse(string keyCode, [NotNullWhen(true)] out Control? control)
    {
        control = keyCode.ToLower() switch
        {
            "ArrowUp" or "w" => Control.Up,
            "ArrowDown" or "s" => Control.Down,
            "ArrowLeft" or "a" => Control.Left,
            "ArrowRight" or "d" => Control.Right,
            "e" => Control.Interact,
            _ => null
        };
        return control is not null;
    }
}

public enum Control
{
    Up,
    Down,
    Left,
    Right,
    Interact,
}
