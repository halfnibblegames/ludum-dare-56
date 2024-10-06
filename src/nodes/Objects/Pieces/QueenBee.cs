using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class QueenBee : Piece
{
    public override int Value => 50;

    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        currentTile.EnumerateAdjacent()
            .Where(c => !ContainsSameColorPiece(board[c]));
}
