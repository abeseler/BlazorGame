using System.Drawing;

namespace BlazorWeb.Logic;

public static class GameState
{
    public const int TILE_SIZE = 64;
    public static GameMap Map { get; set; }
    public static List<Entity> Entities { get; set; }
    public static Queue<Entity> TurnOrder { get; set; }

    static GameState()
    {
        Map = new("Empty Map", new(17, 13));
        TurnOrder = [];
        Entities = [
            new()
            {
                Id = "hero",
                MapPosition = new(1, 0),
                RenderedPosition = new(TILE_SIZE, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/hero.png",
            },
            new()
            {
                Id = "enemy",
                MapPosition = new(3, 0),
                RenderedPosition = new(TILE_SIZE * 3, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/enemy.png",
            },
            new()
            {
                Id = "blue",
                MapPosition = new(5, 0),
                RenderedPosition = new(TILE_SIZE * 5, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/blue.png",
            },
            new()
            {
                Id = "vampire",
                MapPosition = new(7, 0),
                RenderedPosition = new(TILE_SIZE * 7, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/vampire.png",
            },
            new()
            {
                Id = "swashbuckle",
                MapPosition = new(9, 0),
                RenderedPosition = new(TILE_SIZE * 9, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/swashbuckle.png",
            },
            new()
            {
                Id = "angel",
                MapPosition = new(11, 0),
                RenderedPosition = new(TILE_SIZE * 11, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/angel.png",
            },
            new()
            {
                Id = "samuari",
                MapPosition = new(13, 0),
                RenderedPosition = new(TILE_SIZE * 13, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/samuari.png",
            },
            new()
            {
                Id = "black",
                MapPosition = new(15, 0),
                RenderedPosition = new(TILE_SIZE * 15, 0),
                Size = new Size(TILE_SIZE, TILE_SIZE),
                Direction = Direction.Down,
                RenderGroup = RenderGroup.Entity,
                Type = EntityType.NPC,
                Sprite = "assets/black.png",
            },
        ];

        foreach (var entity in Entities)
        {
            Map.Grid[entity.MapPosition.X, entity.MapPosition.Y].Occupant = entity;
            entity.NextActionBehavior = new WanderMapBehavior(entity);
        }
    }

    public static void Update()
    {
        foreach (var entity in Entities) entity.Update();
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
    public Size RenderedSize { get; }
    public GridTile[,] Grid;

    public bool IsInBounds(Point position) =>
        position.X >= 0 && position.X < GridSize.Width && position.Y >= 0 && position.Y < GridSize.Height;

    public bool IsBlocked(Point position)
    {
        return Grid[position.X, position.Y].Occupant is { Collision: CollisionType.Solid };
    }
}

public sealed class GridTile
{
    public Point Position { get; set; }
    public Entity? Occupant { get; set; }
}
