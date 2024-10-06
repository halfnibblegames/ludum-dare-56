using System;
using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class Dragonfly : Piece
{
    public override string Name => "Dragonfly";
    public override string HelpText => "* Moves diagonally.\n* Can move again if they capture.\n* Final boss of every pond.";
    public override int Value => 5;

    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        Enumerable.Empty<TileCoord>()
            .Concat(
                currentTile.EnumerateDiagonal().Where(c => board[c].Piece is null))
            .Concat(potentialCaptures(board, currentTile));

    public override void OnMove(Move move, IMoveSideEffects sideEffects)
    {
        base.OnMove(move, sideEffects);
        var step = move.To.Coord - move.From.Coord;

        // We are always moving diagonally
        if (Math.Abs(step.X) != Math.Abs(step.Y)) throw new InvalidOperationException();

        var distanceMoved = Math.Abs(step.X);

        switch (distanceMoved)
        {
            case 1:
                return;
            case 2:
                var intermediateTileCoord = move.From.Coord + new Step(Math.Sign(step.X), Math.Sign(step.Y));
                var intermediateTile = move.Board[intermediateTileCoord];
                sideEffects.CapturePiece(intermediateTile);
                break;
            default:
                throw new InvalidOperationException("Dragonfly did illegal move");
        }

        // See if there are any turn continuations
        var potentialNewCaptures = potentialCaptures(move.Board, move.To.Coord).ToList();
        if (potentialNewCaptures.Count > 0)
        {
            sideEffects.AllowContinuation(potentialNewCaptures);
        }
    }

    private IEnumerable<TileCoord> potentialCaptures(Board board, TileCoord from)
    {
        return TileCoordExtensions.DiagonalSteps
            .Where(s => (from + s).IsValid() && (from + 2 * s).IsValid())
            .Where(s => board[from + s].Piece is { } piece && piece.IsEnemy != IsEnemy)
            .Select(s => from + 2 * s);
    }
}
