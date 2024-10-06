using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class Ant : Piece
{
    public override int Value => 1;

    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board)
    {
        return Enumerable.Empty<TileCoord>()
            .Concat(
                currentTile.EnumerateOrthogonal().Where(t => board[t].Piece == null))
            .Concat(
                currentTile.EnumerateDiagonal().Where(t => board[t].Piece is { IsEnemy: true }));
    }
}
