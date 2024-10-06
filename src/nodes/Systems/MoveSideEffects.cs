using System;
using System.Collections.Generic;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public interface IMoveSideEffects
{
    void CapturePiece(Tile tile);
    void Ripple(Board board, Tile origin, int rippleRadius);
    void AllowContinuation(List<TileCoord> continuationTiles);
}

public interface IMoveResult
{
    MoveContinuation? Continuation { get; }
    IReadOnlyList<Piece> PiecesCaptured { get; }
}

public abstract class MoveSideSideEffectsBase : IMoveSideEffects
{
    private readonly Move move;

    public MoveContinuation? Continuation { get; private set; }

    protected MoveSideSideEffectsBase(Move move)
    {
        this.move = move;
    }

    public abstract void CapturePiece(Tile tile);

    public abstract void Ripple(Board board, Tile origin, int rippleRadius);

    public void AllowContinuation(List<TileCoord> continuationTiles)
    {
        if (Continuation != null) throw new InvalidOperationException();
        Continuation = new MoveContinuation(
            move.To, move.Piece, continuationTiles.AsReadOnly(), move.PreviousMovesInTurn + 1);
    }
}

public sealed class MoveSideEffects : MoveSideSideEffectsBase
{
    public MoveSideEffects(Move move) : base(move) { }

    public override void CapturePiece(Tile tile)
    {
        if (tile.Piece is null) throw new InvalidOperationException();
        tile.Piece.Destroy();
        tile.Piece = null;
    }

    public override void Ripple(Board board, Tile origin, int rippleRadius)
    {
        origin.AddChild(new BoardRippleAnimation(board, origin, rippleRadius));
    }
}

sealed class MoveSideEffectsPreview : MoveSideSideEffectsBase, IMoveResult
{
    private readonly List<Piece> piecesCaptured = new();

    public IReadOnlyList<Piece> PiecesCaptured { get; }

    public MoveSideEffectsPreview(Move move) : base(move)
    {
        PiecesCaptured = piecesCaptured.AsReadOnly();
    }

    public override void CapturePiece(Tile tile)
    {
        if (tile.Piece is null) return;
        piecesCaptured.Add(tile.Piece);
    }

    public override void Ripple(Board board, Tile origin, int rippleRadius) { }
}
