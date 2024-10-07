using Godot;
using HalfNibbleGame.Autoload;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Cards;

public sealed class MagnifierGlass : CardWithTarget
{
    public MagnifierGlass() : base("Magnifier Glass") { }
    public override Texture GetTexture() => Global.Prefabs.MagnifierGlass!;

    public override string HelpText => "Destroys all bugs in target tile and all tiles around it.";

    protected override IEnumerable<ReachableTile> GetReachableTiles(Board board)
    {
        return board.Tiles.Select(t => t.Coord.Capture());
    }

    protected override Task Use(Board board, Tile target)
    {
        foreach (var coordinate in target.Coord.Yield().Concat(target.Coord.EnumerateAdjacent()))
        {
            var tile = board[coordinate];
            tile.Piece?.Destroy();
            tile.Piece = null;
        }

        return Task.CompletedTask;
    }
}

public static class EnumerableExtensions
{
    public static IEnumerable<T> Yield<T>(this T self)
    {
        yield return self;
    }
}
