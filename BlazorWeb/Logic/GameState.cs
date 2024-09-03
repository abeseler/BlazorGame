using System.Drawing;
using System.Security.Cryptography;

namespace BlazorWeb.Logic;

public static class GameState
{
    public const int TILE_SIZE = 64;
    public static Size MapSize { get; } = new(TILE_SIZE * 17, TILE_SIZE * 13);
    public static HashSet<string> KeysActive { get; } = [];

    public static List<GameObject> GameObjects { get; } = [
        new()
        {
            Id = "hero",
            Position = new Point(TILE_SIZE, 0),
            Size = new Size(TILE_SIZE, TILE_SIZE),
            Direction = Direction.Down,
            Layer = 1,
            Sprite = "assets/hero.png",
        },
        new()
        {
            Id = "enemy",
            Position = new Point(TILE_SIZE * 3, 0),
            Size = new Size(TILE_SIZE, TILE_SIZE),
            Direction = Direction.Down,
            Layer = 1,
            Sprite = "assets/enemy.png",
        },
        new()
        {
            Id = "blue",
            Position = new Point(TILE_SIZE * 5, 0),
            Size = new Size(TILE_SIZE, TILE_SIZE),
            Direction = Direction.Down,
            Layer = 1,
            Sprite = "assets/blue.png",
        },
        new()
        {
            Id = "vampire",
            Position = new Point(TILE_SIZE * 7, 0),
            Size = new Size(TILE_SIZE, TILE_SIZE),
            Direction = Direction.Down,
            Layer = 1,
            Sprite = "assets/vampire.png",
        },
        new()
        {
            Id = "swashbuckle",
            Position = new Point(TILE_SIZE * 9, 0),
            Size = new Size(TILE_SIZE, TILE_SIZE),
            Direction = Direction.Down,
            Layer = 1,
            Sprite = "assets/swashbuckle.png",
        },
        new()
        {
            Id = "angel",
            Position = new Point(TILE_SIZE * 11, 0),
            Size = new Size(TILE_SIZE, TILE_SIZE),
            Direction = Direction.Down,
            Layer = 1,
            Sprite = "assets/angel.png",
        },
        new()
        {
            Id = "samuari",
            Position = new Point(TILE_SIZE * 13, 0),
            Size = new Size(TILE_SIZE, TILE_SIZE),
            Direction = Direction.Down,
            Layer = 1,
            Sprite = "assets/samuari.png",
        },
        new()
        {
            Id = "black",
            Position = new Point(TILE_SIZE * 15, 0),
            Size = new Size(TILE_SIZE, TILE_SIZE),
            Direction = Direction.Down,
            Layer = 1,
            Sprite = "assets/black.png",
        },
    ];

    public static void Update()
    {
        foreach (var gameObject in GameObjects)
        {
            if (gameObject.Position.Y == 0)
            {
                gameObject.Speed = new(0, RandomNumberGenerator.GetInt32(1, 4));
            }                
            else if (gameObject.Position.Y == MapSize.Height - gameObject.Size.Height)
            {
                gameObject.Speed = new(0, RandomNumberGenerator.GetInt32(-4, 0));
            }

            gameObject.Move();
        }
    }
}

public sealed class GameMap
{
    public MapTile[,] Tiles = new MapTile[40,40];
}

public sealed class MapTile
{
    public Guid Id { get; set; }
    public required string Sprite { get; set; }
}

public sealed class GameObject
{
    public required string Id { get; set; }
    public Point Position { get; set; }
    public Point Speed { get; set; }
    public Size Size { get; set; }
    public Direction Direction { get; set; }
    public int Layer { get; set; }
    public required string Sprite { get; set; }
    public void Move()
    {
        var x = Math.Clamp(Position.X + Speed.X, 0, GameState.MapSize.Width - Size.Width);
        var y = Math.Clamp(Position.Y + Speed.Y, 0, GameState.MapSize.Height - Size.Height);
        Position = new Point(x, y);
        Direction = Speed switch
        {
            { Y: > 0 } => Direction.Down,
            { Y: < 0 } => Direction.Up,
            { X: > 0 } => Direction.Right,
            { X: < 0 } => Direction.Left,
            _ => Direction
        };
    }
}
