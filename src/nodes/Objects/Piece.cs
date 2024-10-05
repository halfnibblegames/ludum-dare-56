using System.Collections.Generic;
using System.Linq;
using Godot;

namespace HalfNibbleGame.Objects;

public sealed class Piece : Node2D
{
    public HashSet<TileCoord> ReachableTiles(TileCoord currentTile, Board board)
    {
        return currentTile.EnumerateValidNeighboring().ToHashSet();
    }
}
