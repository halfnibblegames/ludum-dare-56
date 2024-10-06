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
    private NextMovePreview? nextMovePreview;

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

    public virtual MoveOverride? InterruptMove(Move move)
    {
        return null;
    }

    public virtual void OnMove(Move move, IMoveSideEffects sideEffects)
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

        if (isHovered && nextMovePreview is null)
        {
            previewMove();
        }
    }

    private void previewMove()
    {
        var clone = (Piece) Duplicate();
        GetParent().AddChild(clone);
        clone.Modulate = new Color(1, 1, 1, 0.4f);
        var anim = new MoveAnimation(NextMove!.From.Position, NextMove.To.Position, clone);
        GetParent().AddChild(anim);
        nextMovePreview = new NextMovePreview(clone, anim);
    }

    public void StartHover()
    {
        if (isHovered) return;
        isHovered = true;
    }

    public void EndHover()
    {
        if (!isHovered) return;
        isHovered = false;
        finishPreviewAnimation();
    }

    private void finishPreviewAnimation()
    {
        if (nextMovePreview is null) return;
        if (IsInstanceValid(nextMovePreview.Piece))
        {
            nextMovePreview.Piece.QueueFree();
        }
        if (IsInstanceValid(nextMovePreview.Animation))
        {
            nextMovePreview.Animation.QueueFree();
        }
        nextMovePreview = null;
    }

    public void Destroy()
    {
        if (isHovered) EndHover();
        QueueFree();
        Destroyed?.Invoke();
        IsDead = true;
    }

    protected bool ContainsSameColorPiece(Tile tile)
    {
        return tile.Piece is { } piece && piece.IsEnemy == IsEnemy;
    }

    private sealed record NextMovePreview(Piece Piece, MoveAnimation Animation);
}
