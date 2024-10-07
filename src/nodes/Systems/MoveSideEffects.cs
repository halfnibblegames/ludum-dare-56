using System;
using System.Collections.Generic;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects;
using System.Linq;

namespace HalfNibbleGame.Systems;

public interface IMoveSideEffects
{
    void CapturePiece(Board board, Tile tile);
    void Stun(int turnCount);
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

    public abstract void CapturePiece(Board moveBoard, Tile tile);

    public abstract void Stun(int turnCount);

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

    public override void CapturePiece(Board board, Tile tile)
    {
        if (tile.Piece is null) throw new InvalidOperationException();

        if (tile.Piece.RevivesOnDeath)
        {
            var emptyTiles = board.Tiles.Where(x => x.Piece is null).ToList();
            var random = new Random();
            var randomTile = emptyTiles[random.Next(emptyTiles.Count)].Coord;
            //TODO: IF we revive anything that is not a queen we need to figure this out.
            board.AddPiece(Global.Prefabs.QueenBee!.Instance<Piece>(), randomTile);
        }

        tile.Piece.Destroy();
        tile.Piece = null;
    }

    public override void Stun(int turnCount)
    {
        Move.Piece.Stun(turnCount);
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

    public override void CapturePiece(Board board, Tile tile)
    {
        if (tile.Piece is null) return;
        piecesCaptured.Add(tile.Piece);
    }

    public override void Stun(int turnCount)
    {
        StunTime = turnCount;
    }

    public override void PlaySound(Boombox.SoundEffect soundEffect) { }
}
