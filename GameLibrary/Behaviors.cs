using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;

namespace GameLibrary;

public interface IBehavior
{
    IAction DecideNextAction();
}

public sealed class MoveDirection(Entity entity, Direction direction) : IBehavior
{
    private readonly Entity _entity = entity;
    private readonly Direction _direction = direction;
    public IAction DecideNextAction()
    {
        var nextGridY = _direction switch
        {
            Direction.Down => _entity.MapPosition.Y + 1,
            Direction.Up => _entity.MapPosition.Y - 1,
            _ => _entity.MapPosition.Y
        };
        var nextGridX = _direction switch
        {
            Direction.Left => _entity.MapPosition.X - 1,
            Direction.Right => _entity.MapPosition.X + 1,
            _ => _entity.MapPosition.X
        };

        var newPosition = new Point(nextGridX, nextGridY);

        if (GameState.Map.IsInBounds(newPosition) is false || GameState.Map.IsBlocked(newPosition))
        {
            return new WaitAction(0);
        }

        GameState.Map.Grid[nextGridX, nextGridY].Occupant = _entity;

        return new MoveAction(newPosition, 1);
    }
}

public sealed class WanderMapBehavior(Entity entity) : IBehavior
{
    private readonly Entity _entity = entity;
    private Point _destination = entity.MapPosition;
    private Stack<Point>? _path;
    public IAction DecideNextAction()
    {
        if (_path is null || _path.Count == 0)
        {
            var newX = RandomNumberGenerator.GetInt32(0, GameState.Map.GridSize.Width);
            var newY = RandomNumberGenerator.GetInt32(0, GameState.Map.GridSize.Height);
            var destination = new Point(newX, newY);
            if (destination == _entity.MapPosition)
            {
                return new WaitAction(0);
            }
            if (Pathfinder.TryCalculatePath(_entity.MapPosition, destination, out _path) is false)
            {
                return new WaitAction(0);
            }
            _destination = _path.Pop();
        }

        if (GameState.Map.IsBlocked(_path.Peek()))
        {
            _path.Clear();
            return new WaitAction(0);
        }

        _destination = _path.Pop();
        GameState.Map.Grid[_destination.X, _destination.Y].Occupant = _entity;

        return new MoveAction(_destination, 1);
    }
}
