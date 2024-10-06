using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class Grasshopper : Piece
{
    public override int Value => 4;

    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        currentTile.EnumerateKnightMoves()
            .Where(c => !ContainsSameColorPiece(board[c]));

    public override void OnMove(Move move, IMoveSideEffects sideEffects)
    {
        base.OnMove(move, sideEffects);
        if (move.PreviousMovesInTurn == 0)
        {
            sideEffects.AllowContinuation(ReachableTiles(move.To.Coord, move.Board).ToList());
        }
    }
}
