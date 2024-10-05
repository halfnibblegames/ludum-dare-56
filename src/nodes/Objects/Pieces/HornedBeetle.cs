using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class HornedBeetle : Piece
{
    private int stunnedTurnsLeft;

    private static readonly Step[] validSteps = { Step.Up, Step.Left, Step.Right, Step.Down };

    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        validSteps.SelectMany(step =>
            currentTile.EnumerateDirection(step)
                .TakeWhile(c => !ContainsSameColorPiece(board[c]))
                .TakeWhileIncluding(c => board[c].Piece is null));

    public override void OnMove(Move move, MoveSideEffects sideEffects)
    {
        if (move.To.Piece is { } piece && piece.IsEnemy != IsEnemy)
        {
            stunnedTurnsLeft = 2;
            IsStunned = true;
        }
        base.OnMove(move, sideEffects);
    }

    public override void OnTurnStart()
    {
        if (stunnedTurnsLeft == 0)
        {
            IsStunned = false;
        }
        else
        {
            stunnedTurnsLeft--;
        }
    }
}
