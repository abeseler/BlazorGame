using System.Drawing;
using System.Numerics;

namespace GameLibrary;

public interface IAction
{
    void Execute(Entity entity);
}

public sealed class WaitAction(int numOfFrames) : IAction
{
    private int _numOfFrames = numOfFrames;

    public void Execute(Entity entity)
    {
        if (--_numOfFrames <= 0)
            entity.CurrentAction = null;
    }
}

public sealed class MoveAction(Point destination, float speed) : IAction
{
    private readonly Point _destination = destination;
    private readonly Vector2 _renderingDestination = new(destination.X * GameState.TILE_SIZE, destination.Y * GameState.TILE_SIZE);
    private readonly float _speed = speed;
    public void Execute(Entity entity)
    {
        var positionAdjustment = new Vector2(_destination.X - entity.MapPosition.X, _destination.Y - entity.MapPosition.Y) * _speed;        
        var newPosition = Vector2.Add(entity.RenderedPosition, positionAdjustment);

        var distance = Vector2.Distance(entity.RenderedPosition, _renderingDestination);
        entity.RenderedPosition = distance < _speed ? _renderingDestination : newPosition;
        entity.Direction = positionAdjustment switch
        {
            { X: 0, Y: > 0 } => Direction.Down,
            { X: 0, Y: < 0 } => Direction.Up,
            { X: > 0, Y: 0 } => Direction.Right,
            { X: < 0, Y: 0 } => Direction.Left,
            _ => entity.Direction
        };

        if (entity.RenderedPosition == _renderingDestination)
        {
            GameState.Map.Grid[entity.MapPosition.X, entity.MapPosition.Y].Occupant = null;
            entity.MapPosition = _destination;
            entity.CurrentAction = null;
        }
    }
}
