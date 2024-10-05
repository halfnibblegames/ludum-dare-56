using System.Collections.Generic;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class PrayingMantis : Piece
{
    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board)
        => currentTile.EnumerateValidNeighboring();
}
