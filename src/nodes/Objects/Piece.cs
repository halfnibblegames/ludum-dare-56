using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

public abstract class Piece : Node2D
{
    public delegate void PieceDestroyedEventHandler();

    public Move? NextMove { get; set; }

    private bool isEnemy;
    public bool IsEnemy
    {
        get => isEnemy;
        set
        {
            isEnemy = value;
            GetNode<AnimatedSprite>("AnimatedSprite").Animation = isEnemy ? "Dark" : "Light";
        }
    }

    public bool IsStunned { get; protected set; }
    public bool IsDead { get; private set; }

    public event PieceDestroyedEventHandler? Destroyed;

    public abstract IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board);

    public virtual Task<MoveContinuation?>? InterruptMove(Move move, MoveSideEffects sideEffects)
    {
        return null;
    }

    public virtual void OnMove(Move move, MoveSideEffects sideEffects)
    {
        if (move.To.Piece is { } piece && piece.IsEnemy != IsEnemy)
        {
            sideEffects.CapturePiece(move.To);
        }
    }

    public virtual void OnTurnStart() { }

    public void Destroy()
    {
        QueueFree();
        Destroyed?.Invoke();
        IsDead = true;
    }

    protected bool ContainsSameColorPiece(Tile tile)
    {
        return tile.Piece is { } piece && piece.IsEnemy == IsEnemy;
    }
}
