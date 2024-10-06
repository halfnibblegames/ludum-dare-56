using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class Ant : Piece
{
    public override int Value => 1;

    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board)
    {
        return Enumerable.Empty<TileCoord>()
            .Concat(
                currentTile.EnumerateOrthogonal().Where(t => board[t].Piece == null))
            .Concat(
                currentTile.EnumerateDiagonal().Where(t => board[t].Piece is { IsEnemy: true }));
    }

    public override string Name => "Ant";

    public override string GetHelpText() =>
        "* Moves up to one adjacent space.\n* Can only capture diagonally.\n* Lifts heavy weights to suppress the voices in their head.";
}
