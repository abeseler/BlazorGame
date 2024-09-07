using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace BlazorWeb.Logic;

public interface IAction
{
    void Execute(Entity entity);
}

public sealed class WaitAction(TimeSpan duration) : IAction
{
    private readonly long _waitStarted = Stopwatch.GetTimestamp();
    private readonly TimeSpan _duration = duration;

    public void Execute(Entity entity)
    {
        if (Stopwatch.GetElapsedTime(_waitStarted) > _duration)
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

        if (entity.RenderedPosition == _renderingDestination)
        {
            GameState.Map.Grid[entity.MapPosition.X, entity.MapPosition.Y].Occupant = null;
            entity.MapPosition = _destination;
            entity.CurrentAction = null;
        }
    }
}
