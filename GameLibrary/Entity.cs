using System.Drawing;
using System.Numerics;

namespace GameLibrary;

public sealed class Entity
{
    public required string Id { get; set; }
    public required string Sprite { get; set; }
    public Point MapPosition { get; set; }
    public Vector2 RenderedPosition { get; set; }
    public Size Size { get; set; }
    public Direction Direction { get; set; }
    public EntityType Type { get; set; }
    public CollisionType Collision { get; set; }
    public RenderGroup RenderGroup { get; set; }
    public IAction? CurrentAction { get; set; }
    public IBehavior? NextActionBehavior { get; set; }

    public void Update()
    {
        CurrentAction ??= NextActionBehavior?.DecideNextAction();
        CurrentAction?.Execute(this);
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
