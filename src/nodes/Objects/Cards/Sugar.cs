using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame.Objects.Cards;

public sealed class Sugar : CardWithTarget
{
    public override Texture GetTexture() => Global.Prefabs.Sugar!;

    public Sugar() : base("Sugar Cube")
    {
    }

    protected override IEnumerable<ReachableTile> GetReachableTiles(Board board)
    {
        return board.Tiles.Where(t => t.Piece is { IsEnemy: false })
            .Select(t => t.Coord.MoveTo());
    }

    protected override Task Use(Board board, Tile target)
    {
        throw new System.NotImplementedException();
    }

    public override string HelpText => "The next time target bug moves, it gets to move twice.\n[b]- The children yearn for the sweets.[/b]";
}
