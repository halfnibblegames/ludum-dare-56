using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

public abstract class Piece : Node2D
{
    public delegate void PieceDestroyedEventHandler();

    public Move? NextMove { get; set; }

    private AnimatedSprite sprite => GetNode<AnimatedSprite>("AnimatedSprite");

    private bool isHovered;

    private bool isEnemy;
    public bool IsEnemy
    {
        get => isEnemy;
        set
        {
            isEnemy = value;
            sprite.Animation = isEnemy ? "Dark" : "Light";
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

    public override void _Process(float delta)
    {
        if (NextMove is null)
        {
            sprite.Position = Vector2.Zero;
            return;
        }

        var offset = 0.5f + 0.5f * Mathf.Sin(0.03f * OS.GetTicksMsec());
        sprite.Position = offset * Vector2.Right;
    }

    public void StartHover()
    {
        isHovered = true;
    }

    public void EndHover()
    {
        isHovered = false;
    }

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
