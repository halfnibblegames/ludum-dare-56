using System;
using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class PrayingMantis : Piece
{
    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        currentTile.EnumerateAdjacent()
            .Where(c => !ContainsSameColorPiece(board[c]));

    public override bool InterruptMove(Move move, MoveSideEffects sideEffects)
    {
        if (move.To.Piece is null)
        {
            return false;
        }

        var step = move.To.Coord - move.From.Coord;
        var index = Array.IndexOf(TileCoordExtensions.Steps, step);
        if (index < 0) throw new Exception();

        var stepCount = TileCoordExtensions.Steps.Length;
        var nextStep = TileCoordExtensions.Steps[(index + 1) % stepCount];
        var prevStep = TileCoordExtensions.Steps[(stepCount + index - 1) % stepCount];
        var nextTile = move.Board[move.From.Coord + nextStep];
        var prevTile = move.Board[move.From.Coord + prevStep];

        foreach (var tile in new[] { move.From, nextTile, prevTile })
        {
            if (tile.Piece is not null && tile.Piece.IsEnemy != IsEnemy)
            {
                sideEffects.CapturePiece(tile);
            }
        }

        // TODO: play animation

        return true;
    }

    public override void OnMove(Move move, MoveSideEffects sideEffects)
    {
        sideEffects.Ripple(move.Board, move.To, 2);
    }
}
