using System.Collections.Generic;
using Godot;

namespace HalfNibbleGame.Objects;

public abstract class Piece : Node2D
{
    public abstract IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board);
}
