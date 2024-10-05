using System.Threading.Tasks;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public static class Moves
{
    public static IMove MovePiece(Piece piece, Tile from, Tile to) => new MovePieceMove(piece, from, to);
    public static IMove Capture(Piece capturingPiece, Tile from, Tile to) => new CapturePieceMove(capturingPiece, from, to);

    private sealed record MovePieceMove(Piece Piece, Tile From, Tile To) : MoveBase(Piece, From, To)
    {
        protected override void ExecuteLogic()
        {
            From.Piece = null;
            To.Piece = Piece;
            Piece.Position = To.Position;
        }
    }

    private sealed record CapturePieceMove(Piece Piece, Tile From, Tile To) : MoveBase(Piece, From, To)
    {
        protected override void ExecuteLogic()
        {
            var pieceToCapture = To.Piece;
            pieceToCapture?.Destroy();
            From.Piece = null;
            To.Piece = Piece;
            Piece.Position = To.Position;
        }
    }

    private abstract record MoveBase(Piece Piece, Tile From, Tile To) : IMove
    {
        public async Task Execute()
        {
            var anim = new MoveAnimation(From.Position, To.Position, Piece);
            var signal = Piece.ToSignal(anim, nameof(MoveAnimation.Finished));
            Piece.AddChild(anim);

            await signal;

            ExecuteLogic();
        }

        protected abstract void ExecuteLogic();
    }
}

public interface IMove
{
    Task Execute();
}
