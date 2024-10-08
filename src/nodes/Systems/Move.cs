﻿using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed record Move(Board Board, Piece Piece, Tile From, Tile To, int PreviousMovesInTurn)
{
    public bool Validate()
    {
        // Piece has died
        if (Piece.IsDead) return false;

        // Piece is stunned
        if (Piece.IsStunned) return false;

        // Piece cannot reach the target
        if (!Piece.ReachableTiles(From.Coord, Board).Select(rt => rt.Coord).ToHashSet().Contains(To.Coord))
        {
            return false;
        }

        // Can always move to an empty tile
        if (To.Piece is null) return true;

        // Cannot capture your own pieces
        if (To.Piece.IsEnemy == Piece.IsEnemy) return false;

        return true;
    }

    public async Task<MoveContinuation?> Execute()
    {
        var sideEffects = new MoveSideEffects(this);

        if (Piece.InterruptMove(this) is { } moveOverride)
        {
            await moveOverride.AnimationFactory();
            moveOverride.Execute(this, sideEffects);
            return sideEffects.Continuation;
        }

        var anim = new MoveAnimation(From.Position, To.Position, Piece);

        var signal = Piece.ToSignal(anim, nameof(MoveAnimation.Finished));
        Piece.AddChild(anim);

        if (!Piece.IsEnemy)
        {
            sideEffects.PlaySound(Piece.MovementEffect);
        }
        await signal;

        doLogic(sideEffects);
        return sideEffects.Continuation;
    }

    public IMoveResult Preview()
    {
        var sideEffects = new MoveSideEffectsPreview(this);
        if (Piece.InterruptMove(this) is { } moveOverride)
        {
            moveOverride.Execute(this, sideEffects);
            return sideEffects;
        }

        Piece.OnMove(this, sideEffects);
        return sideEffects;
    }

    private void doLogic(MoveSideEffects sideEffects)
    {
        Piece.OnMove(this, sideEffects);
        From.Piece = null;
        To.Piece = Piece;
        Piece.Position = To.Position;
    }
}
