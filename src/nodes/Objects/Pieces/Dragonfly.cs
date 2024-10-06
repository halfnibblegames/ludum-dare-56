using System;
using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class Dragonfly : Piece
{
    public override string Name => "Dragonfly";

    public override string GetHelpText() =>
        "* Moves diagonally.\n* Can move again if they capture.\n* Final boss of every pond.";

    public override int Value => 5;

    public override IEnumerable<ReachableTile> ReachableTiles(TileCoord currentTile, Board board)
    {
        foreach (var dir in TileCoordExtensions.DiagonalSteps)
        {
            var current = currentTile + dir;

            while (current.IsValid() && !ContainsSameColorPiece(board[current]))
            {
                var tile = board[current];
                if (tile.Piece is null)
                {
                    yield return current.MoveTo();
                    current += dir;
                    continue;
                }

                var next = current + dir;
                if (next.IsValid() && board[next].Piece is null)
                {
                    yield return next.Capture();
                }
                break;
            }
        }
    }

    public override void OnMove(Move move, IMoveSideEffects sideEffects)
    {
        base.OnMove(move, sideEffects);
        var step = move.To.Coord - move.From.Coord;

        // We are always moving diagonally
        if (Math.Abs(step.X) != Math.Abs(step.Y)) throw new InvalidOperationException();

        var distanceMoved = Math.Abs(step.X);
        if (distanceMoved <= 1)
        {
            return;
        }

        var dir = new Step(Math.Sign(step.X), Math.Sign(step.Y));
        var lastTile = move.To.Coord - dir;
        if (move.Board[lastTile].Piece is null)
        {
            return;
        }

        sideEffects.CapturePiece(move.Board[lastTile]);

        // See if there are any turn continuations
        var newMoves = new List<TileCoord>();
        foreach (var nextDir in TileCoordExtensions.DiagonalSteps)
        {
            // Cannot move backwards
            if (nextDir == -dir) continue;

            var current = move.To.Coord + nextDir;

            while (current.IsValid() && !ContainsSameColorPiece(move.Board[current]))
            {
                var tile = move.Board[current];
                if (tile.Piece is null)
                {
                    current += nextDir;
                    continue;
                }

                var next = current + nextDir;
                if (next.IsValid() && move.Board[next].Piece is null)
                {
                    newMoves.Add(next);
                }
                break;
            }
        }

        if (newMoves.Count > 0)
        {
            sideEffects.AllowContinuation(newMoves.Select(t => t.Capture()).ToList());
        }
    }
}
