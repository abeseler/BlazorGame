using System.Drawing;
using System.Security.Cryptography;

namespace BlazorWeb.Logic;

public static class GameState
{
    public const int TILE_SIZE = 64;
    public static GameMap Map { get; set; }
    public static List<GameObject> Entities { get; set; }
    public static Queue<GameObject> TurnOrder { get; set; }

    static GameState()
    {
        Map = new("Empty Map", new(17, 13));
        TurnOrder = [];
        Entities = [
            new()
            {
                Id = "hero",
                GridPosition = new(1, 0),
                RenderedPosition = new Point(TILE_SIZE, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.Player,
                Sprite = "assets/hero.png",
            },
            new()
            {
                Id = "enemy",
                GridPosition = new(3, 0),
                RenderedPosition = new Point(TILE_SIZE * 3, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/enemy.png",
            },
            new()
            {
                Id = "blue",
                GridPosition = new(5, 0),
                RenderedPosition = new Point(TILE_SIZE * 5, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/blue.png",
            },
            new()
            {
                Id = "vampire",
                GridPosition = new(7, 0),
                RenderedPosition = new Point(TILE_SIZE * 7, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/vampire.png",
            },
            new()
            {
                Id = "swashbuckle",
                GridPosition = new(9, 0),
                RenderedPosition = new Point(TILE_SIZE * 9, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/swashbuckle.png",
            },
            new()
            {
                Id = "angel",
                GridPosition = new(11, 0),
                RenderedPosition = new Point(TILE_SIZE * 11, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.Player,
                Sprite = "assets/angel.png",
            },
            new()
            {
                Id = "samuari",
                GridPosition = new(13, 0),
                RenderedPosition = new Point(TILE_SIZE * 13, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/samuari.png",
            },
            new()
            {
                Id = "black",
                GridPosition = new(15, 0),
                RenderedPosition = new Point(TILE_SIZE * 15, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/black.png",
            },
        ];

        foreach (var entity in Entities)
        {
            Map.Grid[entity.GridPosition.X, entity.GridPosition.Y].Occupant = entity;
        }
    }

    public static void Update()
    {
        foreach (var gameObject in Entities.Where(obj => obj.Type == EntityType.NPC))
        {
            var nextGridX = Math.Clamp(gameObject.Direction switch
            {
                Direction.Left => gameObject.GridPosition.X - 1,
                Direction.Right => gameObject.GridPosition.X + 1,
                _ => gameObject.GridPosition.X
            }, 0 , Map.GridSize.Width - 1);
            var nextGridY = Math.Clamp(gameObject.Direction switch
            {
                Direction.Up => gameObject.GridPosition.Y - 1,
                Direction.Down => gameObject.GridPosition.Y + 1,
                _ => gameObject.GridPosition.Y
            }, 0, Map.GridSize.Height - 1);

            Map.Grid[nextGridX, nextGridY].Occupant = gameObject;

            if (gameObject.RenderedPosition.Y == 0 || gameObject.RenderedPosition.X == 0)
            {
                gameObject.Speed = new(0, RandomNumberGenerator.GetInt32(1, 4));
            }                
            else if (gameObject.RenderedPosition.Y == Map.RenderedSize.Height - gameObject.Size.Height || gameObject.RenderedPosition.X == Map.RenderedSize.Width - gameObject.Size.Width)
            {
                gameObject.Speed = new(0, RandomNumberGenerator.GetInt32(1, 4) * -1);
            }

            gameObject.Move();

            if (gameObject.RenderedRect.IntersectsWith(gameObject.GridPositionRect) is false)
            {
                Map.Grid[gameObject.GridPosition.X, gameObject.GridPosition.Y].Occupant = null;
                gameObject.GridPosition = new(nextGridX, nextGridY);
            }
        }
    }
}

public sealed class GameMap
{
    public GameMap(string name, Size size)
    {
        Name = name;
        GridSize = size;
        RenderedSize = new(size.Width * GameState.TILE_SIZE, size.Height * GameState.TILE_SIZE);
        Grid = new GridTile[size.Width, size.Height];

        for (int x = 0; x < size.Width; x++)
        {
            for (int y = 0; y < size.Height; y++)
            {
                Grid[x, y] = new GridTile
                {
                    Position = new(x, y),
                    Occupant = null,
                };
            }
        }
    }
    public string Name { get; }
    public Size GridSize { get; }
    public Size RenderedSize { get; set; }
    public GridTile[,] Grid;
}

public sealed class GridTile
{
    public Point Position { get; set; }
    public GameObject? Occupant { get; set; }
}

public sealed class GameObject
{
    public required string Id { get; set; }
    public Point GridPosition { get; set; }
    public Point RenderedPosition { get; set; }
    public Rectangle RenderedRect => new(RenderedPosition, Size);
    public Rectangle GridPositionRect => new(GridPosition.X * Size.Width, GridPosition.Y * Size.Height, Size.Width, Size.Height);
    public Point Speed { get; set; }
    public Size Size { get; set; }
    public Direction Direction { get; set; }
    public EntityType Type { get; set; }
    public CollisionType Collision { get; set; }
    public RenderGroup RenderGroup { get; set; }
    public required string Sprite { get; set; }
    public void Move()
    {
        var x = Math.Clamp(RenderedPosition.X + Speed.X, 0, GameState.Map.RenderedSize.Width - Size.Width);
        var y = Math.Clamp(RenderedPosition.Y + Speed.Y, 0, GameState.Map.RenderedSize.Height - Size.Height);
        RenderedPosition = new Point(x, y);
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

public enum Direction
{
    Down = 0,
    Left = 1,
    Right = 2,
    Up = 3
}

public enum RenderGroup
{
    Background,
    Entity,
    Foreground,
    UI,
}

public enum CollisionType
{
    Solid,
    Passable,
}

public enum EntityType
{
    Player,
    NPC,
    Effects,
}
