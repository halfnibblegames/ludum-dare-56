using System;
using System.Collections.Generic;
using Godot;

namespace HalfNibbleGame.Objects;

public abstract class Piece : Node2D
{
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

    public abstract IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board);

    public void Destroy()
    {
        QueueFree();
    }

    protected bool ContainsSameColorPiece(Tile tile)
    {
        return tile.Piece is { } piece && piece.IsEnemy == IsEnemy;
    }
}
