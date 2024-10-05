using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class PrayingMantis : Piece
{
    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        currentTile.EnumerateAdjacent()
            .Where(c => !ContainsSameColorPiece(board[c]));

    public override void OnMove(Move move, MoveSideEffects sideEffects)
    {
        base.OnMove(move, sideEffects);
        sideEffects.Ripple(move.Board, move.To, 2);
    }
}
