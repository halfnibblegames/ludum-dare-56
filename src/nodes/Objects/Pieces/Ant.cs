using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class Ant : Piece
{
    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board)
    {
        return Enumerable.Empty<TileCoord>()
            .Concat(
                currentTile.EnumerateValidAdjacent().Where(t => board[t].Piece == null))
            .Concat(
                currentTile.EnumerateValidDiagonal().Where(t => board[t].Piece is { IsEnemy: true }));
    }
}
