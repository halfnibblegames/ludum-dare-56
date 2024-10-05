using System;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class MoveSideEffects
{
    public void CapturePiece(Tile tile)
    {
        if (tile.Piece is null) throw new InvalidOperationException();
        tile.Piece.Destroy();
        tile.Piece = null;
    }
}
