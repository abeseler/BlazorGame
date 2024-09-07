using System.Drawing;
using System.Security.Cryptography;

namespace BlazorWeb.Logic;

public interface IBehavior
{
    IAction DecideNextAction();
}

public sealed class MoveUpDownBehavior(Entity entity) : IBehavior
{
    private readonly Entity _entity = entity;
    private int _speed = RandomNumberGenerator.GetInt32(1, 4);
    public IAction DecideNextAction()
    {
        if (_entity.MapPosition.Y == 0 || _entity.MapPosition.Y == GameState.Map.GridSize.Height - 1)
        {
            _entity.Direction = _entity.MapPosition.Y == 0 ? Direction.Down : Direction.Up;
            _speed = RandomNumberGenerator.GetInt32(1, 4);
        }

        var nextGridY = _entity.Direction == Direction.Down ? _entity.MapPosition.Y + 1 : _entity.MapPosition.Y - 1;
        var nextGridX = _entity.MapPosition.X;

        if (GameState.Map.Grid[nextGridX, nextGridY].Occupant is not null)
        {
            return new WaitAction(100);
        }

        GameState.Map.Grid[nextGridX, nextGridY].Occupant = _entity;

        return new MoveAction(new(nextGridX, nextGridY), _speed);
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