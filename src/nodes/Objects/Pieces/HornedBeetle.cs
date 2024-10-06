using System;
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
                .TakeWhile(c => !ContainsSameColorPiece(board[c])));

    public override void OnMove(Move move, MoveSideEffects sideEffects)
    {
        var diff = move.To.Coord - move.From.Coord;
        var singleStep = new Step(Math.Sign(diff.X), Math.Sign(diff.Y));
        var traversedTiles = move.From.Coord.EnumerateDirection(singleStep).TakeWhileIncluding(t => t != move.To.Coord);

        var captured = false;
        foreach (var traversedTile in traversedTiles)
        {
            var boardTile = move.Board[traversedTile];
            if (boardTile.Piece is { } piece && piece.IsEnemy != IsEnemy)
            {
                sideEffects.CapturePiece(boardTile);
                captured = true;
            }
        }

        if (captured)
        {
            stunnedTurnsLeft = 2;
            IsStunned = true;
        }
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
