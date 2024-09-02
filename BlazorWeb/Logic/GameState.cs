using System.Drawing;
using System.Security.Cryptography;

namespace BlazorWeb.Logic;

public static class GameState
{
    public static Size MapSize { get; } = new(1088, 832);
    public static HashSet<string> KeysActive { get; } = [];

    public static List<GameObject> GameObjects { get; } = [
        new()
        {
            Id = "hero",
            Speed = 2,
            Position = new Point(64, 0),
            Size = new Size(64, 64),
            Direction = Direction.Up,
            Layer = 1,
            Sprite = "assets/hero.png",
        },
        new()
        {
            Id = "enemy",
            Speed = 2,
            Position = new Point(192, 0),
            Size = new Size(64, 64),
            Direction = Direction.Down,
            Layer = 1,
            Sprite = "assets/enemy.png",
        },
        new()
        {
            Id = "blue",
            Speed = 2,
            Position = new Point(320, 0),
            Size = new Size(64, 64),
            Direction = Direction.Down,
            Layer = 1,
            Sprite = "assets/blue.png",
        }
    ];

    public static void Update()
    {
        foreach (var gameObject in GameObjects)
        {
            if (gameObject.Position.Y == 0)
            {
                gameObject.Direction = Direction.Down;
                gameObject.Speed = RandomNumberGenerator.GetInt32(1, 4);
            }                
            else if (gameObject.Position.Y == MapSize.Height - gameObject.Size.Height)
            {
                gameObject.Direction = Direction.Up;
                gameObject.Speed = RandomNumberGenerator.GetInt32(1, 4);
            }                

            gameObject.Move(gameObject.Direction, MapSize);
        }
    }
}

public sealed class GameObject
{
    public required string Id { get; set; }
    public Point Coordinate { get; set; }
    public Point Position { get; set; }
    public Size Size { get; set; }
    public Direction Direction { get; set; }
    public int Speed { get; set; }
    public int Layer { get; set; }
    public required string Sprite { get; set; }
    public void Move(Direction direction, Size map)
    {
        Position = direction switch
        {
            Direction.Up => new Point(Position.X, Math.Clamp(Position.Y - Speed, 0, map.Height - Size.Height)),
            Direction.Down => new Point(Position.X, Math.Clamp(Position.Y + Speed, 0, map.Height - Size.Height)),
            Direction.Left => new Point(Math.Clamp(Position.X - Speed, 0, map.Width - Size.Width), Position.Y),
            Direction.Right => new Point(Math.Clamp(Position.X + Speed, 0, map.Width - Size.Width), Position.Y),
            _ => Position
        };
    }
}
