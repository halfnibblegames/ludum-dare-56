﻿using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

public abstract class Piece : Node2D, IHelpable
{
    public delegate void PieceDestroyedEventHandler();

    public Move? NextMove { get; set; }

    private AnimatedSprite sprite => GetNode<AnimatedSprite>("AnimatedSprite");

    private bool isHovered;
    private NextMovePreview? nextMovePreview;
    protected int StunnedTurnsLeft { get; private set; }

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

    public bool IsStunned { get; private set; }
    public bool IsDead { get; private set; }

    public event PieceDestroyedEventHandler? Destroyed;

    public abstract int Value { get; }

    public abstract IEnumerable<ReachableTile> ReachableTiles(TileCoord currentTile, Board board);

    public Boombox.SoundEffect MovementEffect { get; protected set; } = Boombox.SoundEffect.Walk;

    public abstract string Name { get; }
    public abstract string GetHelpText();

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

    public void OnTurnStart()
    {
        var stunLabel = GetNodeOrNull<Label>("Stun");
        if (stunLabel is null) return;

        if (StunnedTurnsLeft == 0)
        {
            IsStunned = false;
            stunLabel.Text = "";
        }
        else
        {
            stunLabel.Text = (StunnedTurnsLeft--).ToString();
        }
    }

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
            SpawnMovePreview(NextMove);
        }
    }

    public void SpawnMovePreview(Move move)
    {
        EndMovePreview();

        var clone = (Piece) Duplicate();
        clone.ZIndex += 2;
        GetParent().AddChild(clone);
        clone.Modulate = new Color(1, 1, 1, 0.4f);
        var anim = new MoveAnimation(move.From.Position, move.To.Position, clone);
        GetParent().AddChild(anim);
        nextMovePreview = new NextMovePreview(clone, anim);
    }

    public void EndMovePreview()
    {
        if (nextMovePreview is not null)
        {
            finishPreviewAnimation();
        }
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
        if (IsEnemy)
        {
            finishPreviewAnimation();
        }
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

    public void Stun(int stunnedTurns)
    {
        StunnedTurnsLeft = stunnedTurns;
        IsStunned = true;
    }

    public void Destroy()
    {
        if (isHovered) EndHover();
        Global.Services.Get<ShakeCamera>().Shake(7.5f);
        if (Global.Prefabs.CaptureExplosion?.Instance<Node2D>() is { } node)
        {
            node.Position = Position;
            GetParent().AddChild(node);
        }
        Global.Services.Get<Boombox>().Play(Boombox.SoundEffect.Capture);
        QueueFree();
        Destroyed?.Invoke();
        IsDead = true;
    }

    protected bool ContainsSameColorPiece(Tile tile)
    {
        return tile.Piece is { } piece && piece.IsEnemy == IsEnemy;
    }

    protected ReachableTile MoveOrCapture(Tile tile) => MoveOr(tile, TileAction.Capture);

    protected ReachableTile MoveOr(Tile tile, TileAction action) =>
        tile.Piece == null ? tile.Coord.MoveTo() : new ReachableTile(tile.Coord, action);

    private sealed record NextMovePreview(Piece Piece, MoveAnimation Animation);
}
