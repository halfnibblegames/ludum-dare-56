using System;
using System.Collections.Generic;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class MoveSideEffects
{
    private readonly Move move;

    public MoveContinuation? Continuation { get; private set; }

    public MoveSideEffects(Move move)
    {
        this.move = move;
    }

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

    public void AllowContinuation(List<TileCoord> continuationTiles)
    {
        if (Continuation != null) throw new InvalidOperationException();
        Continuation = new MoveContinuation(
            move.To, move.Piece, continuationTiles.AsReadOnly(), move.PreviousMovesInTurn + 1);
    }
}
