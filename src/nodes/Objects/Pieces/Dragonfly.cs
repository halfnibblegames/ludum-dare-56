using System;
using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class Dragonfly : Piece
{
    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        Enumerable.Empty<TileCoord>()
            .Concat(
                currentTile.EnumerateDiagonal().Where(c => board[c].Piece is null))
            .Concat(
                TileCoordExtensions.DiagonalSteps
                    .Where(s => (currentTile + s).IsValid() && (currentTile + 2 * s).IsValid())
                    .Where(s => board[currentTile + s].Piece is { } piece && piece.IsEnemy != IsEnemy)
                    .Select(s => currentTile + 2 * s));

    public override void OnMove(Board board, Tile fromTile, Tile toTile, MoveSideEffects sideEffects)
    {
        base.OnMove(board, fromTile, toTile, sideEffects);
        var step = toTile.Coord - fromTile.Coord;

        // We are always moving diagonally
        if (Math.Abs(step.X) != Math.Abs(step.Y)) throw new InvalidOperationException();

        var distanceMoved = Math.Abs(step.X);

        switch (distanceMoved)
        {
            case 1:
                break;
            case 2:
                var intermediateTileCoord = fromTile.Coord + new Step(Math.Sign(step.X), Math.Sign(step.Y));
                var intermediateTile = board[intermediateTileCoord];
                sideEffects.CapturePiece(intermediateTile);
                break;
            default:
                throw new InvalidOperationException("Dragonfly did illegal move");
        }
    }
}
