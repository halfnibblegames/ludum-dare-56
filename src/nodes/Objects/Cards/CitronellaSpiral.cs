using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Autoload;
using System.Threading.Tasks;

namespace HalfNibbleGame.Objects.Cards;

public sealed class CitronellaSpiral : CardWithTarget
{
    public CitronellaSpiral() : base("Citronella Spiral") { }
    public override Texture GetTexture() => Global.Prefabs.CitronellaSpiral!;

    public override string HelpText => "Target can't be captured until the end of the your next turn.\n[b]Do these things even work?[/b]";

    protected override IEnumerable<ReachableTile> GetReachableTiles(Board board)
    {
        return board.Tiles.Where(t => t.Piece is { IsEnemy: false })
            .Select(t => t.Coord.MoveTo());
    }

    protected override Task Use(Board board, Tile target)
    {
        //TODO: Implement
        return Task.CompletedTask;
    }
}
