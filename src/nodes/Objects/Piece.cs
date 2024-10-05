using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

public abstract class Piece : Node2D
{
    public delegate void PieceDestroyedEventHandler();

    private bool isEnemy = false;
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

    public virtual void OnMove(Board board, Tile fromTile, Tile toTile, MoveSideEffects sideEffects)
    {
        if (toTile.Piece is { } piece && piece.IsEnemy != IsEnemy)
        {
            sideEffects.CapturePiece(toTile);
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
