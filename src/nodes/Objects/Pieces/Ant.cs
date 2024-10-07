using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class Ant : Piece
{
    public override int Value => 1;

    public override IEnumerable<ReachableTile> ReachableTiles(TileCoord currentTile, Board board)
    {
        return Enumerable.Empty<ReachableTile>()
            .Concat(
                currentTile.EnumerateOrthogonal()
                    .Where(t => board[t].Piece == null)
                    .Select(t => t.MoveTo()))
            .Concat(
                currentTile.EnumerateDiagonal()
                    .Where(t => board[t].Piece is { } piece && piece.IsEnemy != IsEnemy)
                    .Select(t => t.Capture()));
    }

    public override string DisplayName => "Ant";

    public override string HelpText => "* Moves up to one adjacent space.\n* Can only capture diagonally.\n* Lifts heavy weights to suppress the voices in their head.";
}
