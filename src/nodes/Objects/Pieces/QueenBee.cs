using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class QueenBee : Piece
{
    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        currentTile.EnumerateValidNeighboring()
            .Where(c => !ContainsSameColorPiece(board[c]));
}
