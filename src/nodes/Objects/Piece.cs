using System.Collections.Generic;
using System.Linq;
using Godot;

namespace HalfNibbleGame.Objects;

public sealed class Piece : Node2D
{
    public IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board)
    {
        return currentTile.EnumerateValidNeighboring();
        // return currentTile.EnumerateStepsWhileValid(Step.UpRight)
        //     .Concat(currentTile.EnumerateStepsWhileValid(Step.DownLeft));
    }
}
