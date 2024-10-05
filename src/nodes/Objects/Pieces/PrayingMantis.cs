using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class PrayingMantis : Piece
{
    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        currentTile.EnumerateValidNeighboring()
            .Where(c => !ContainsSameColorPiece(board[c]));

    public override void OnMove(Board board, Tile fromTile, Tile toTile, MoveSideEffects sideEffects)
    {
        base.OnMove(board, fromTile, toTile, sideEffects);
        sideEffects.Ripple(board, toTile, 2);
    }
}
