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

    public void Ripple(Board board, Tile origin, int rippleRadius)
    {
        origin.AddChild(new BoardRippleAnimation(board, origin, rippleRadius));
    }
}
