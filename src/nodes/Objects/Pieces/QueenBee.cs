using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class QueenBee : Piece
{
    public override string Name => "Queen Bee";

    public override string GetHelpText() =>
        "* Can move to any neighbor tile.\n* Can capture in any direction.\n* Must, literally, be defended at all costs: The game ends if they are captured.";

    public override int Value => 50;

    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        currentTile.EnumerateAdjacent()
            .Where(c => !ContainsSameColorPiece(board[c]));
}
