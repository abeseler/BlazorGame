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
            return new WaitAction(TimeSpan.FromMilliseconds(500));
        }

        GameState.Map.Grid[nextGridX, nextGridY].Occupant = _entity;

        return new MoveAction(new(nextGridX, nextGridY), _speed);
    }
}
