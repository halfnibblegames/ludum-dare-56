using System.Collections.Generic;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class QueenBee : Piece
{
    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board)
    {
        return currentTile.EnumerateValidNeighboring();
    }
}
