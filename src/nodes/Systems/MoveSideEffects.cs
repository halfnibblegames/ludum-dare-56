using System;
using System.Collections.Generic;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public interface IMoveSideEffects
{
    void CapturePiece(Tile tile);
    void Stun(int turnCount);
    void Ripple(Board board, Tile origin, int rippleRadius);
    void AllowContinuation(List<ReachableTile> continuationTiles);
    void PlaySound(Boombox.SoundEffect soundEffect);
}

public interface IMoveResult
{
    MoveContinuation? Continuation { get; }
    IReadOnlyList<Piece> PiecesCaptured { get; }
    int StunTime { get; }
}

public abstract class MoveSideSideEffectsBase : IMoveSideEffects
{
    protected readonly Move Move;

    public MoveContinuation? Continuation { get; private set; }

    protected MoveSideSideEffectsBase(Move move)
    {
        Move = move;
    }

    public abstract void CapturePiece(Tile tile);

    public abstract void Stun(int turnCount);

    public abstract void Ripple(Board board, Tile origin, int rippleRadius);
    public abstract void PlaySound(Boombox.SoundEffect soundEffect);

    public void AllowContinuation(List<ReachableTile> continuationTiles)
    {
        if (Continuation != null) throw new InvalidOperationException();
        Continuation = new MoveContinuation(
            Move.To, Move.Piece, continuationTiles.AsReadOnly(), Move.PreviousMovesInTurn + 1);
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

    public override void Stun(int turnCount)
    {
        Move.Piece.Stun(turnCount);
    }

    public override void Ripple(Board board, Tile origin, int rippleRadius)
    {
        origin.AddChild(new BoardRippleAnimation(board, origin, rippleRadius));
    }

    public override void PlaySound(Boombox.SoundEffect soundEffect)
    {
        Global.Services.Get<Boombox>().Play(soundEffect);
    }
}

sealed class MoveSideEffectsPreview : MoveSideSideEffectsBase, IMoveResult
{
    private readonly List<Piece> piecesCaptured = new();

    public int StunTime { get; private set; }
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

    public override void Stun(int turnCount)
    {
        StunTime = turnCount;
    }

    public override void Ripple(Board board, Tile origin, int rippleRadius) { }
    public override void PlaySound(Boombox.SoundEffect soundEffect) { }
}
