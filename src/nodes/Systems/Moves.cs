using System;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

static class Moves
{
    public static IMove MovePiece(Piece piece, Tile from, Tile to) => new MovePieceMove(piece, from, to);

    private sealed record MovePieceMove(Piece Piece, Tile From, Tile To) : IMove
    {
        public bool Validate()
        {
            return From.Piece == Piece && To.Piece == null;
        }

        public void Execute()
        {
            if (!Validate()) throw new Exception();

            From.Piece = null;
            To.Piece = Piece;
            Piece.Position = To.Position;
        }
    }
}

interface IMove
{
    bool Validate();
    void Execute();
}
