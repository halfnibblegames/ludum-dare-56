using System;
using System.Threading.Tasks;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public static class Moves
{
    public static IMove MovePiece(Piece piece, Tile from, Tile to) => new MovePieceMove(piece, from, to);

    private sealed record MovePieceMove(Piece Piece, Tile From, Tile To) : IMove
    {
        public bool Validate()
        {
            return From.Piece == Piece && To.Piece == null;
        }

        public async Task Execute()
        {
            if (!Validate()) throw new Exception();

            var anim = new MoveAnimation(From.Position, To.Position, Piece);
            var signal = Piece.ToSignal(anim, nameof(MoveAnimation.Finished));
            Piece.AddChild(anim);

            await signal;

            From.Piece = null;
            To.Piece = Piece;
            Piece.Position = To.Position;
        }
    }
}

public interface IMove
{
    bool Validate();
    Task Execute();
}
