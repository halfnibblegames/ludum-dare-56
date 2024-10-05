using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class Grasshopper : Piece
{
    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        currentTile.EnumerateKnightMoves()
            .Where(c => !ContainsSameColorPiece(board[c]));
}
