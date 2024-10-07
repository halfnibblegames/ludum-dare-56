using Godot;
using HalfNibbleGame.Autoload;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalfNibbleGame.Objects.Cards;

public sealed class MagnifierGlass : CardWithTarget
{
    public MagnifierGlass() : base("Magnifier Glass") { }
    public override Texture GetTexture() => Global.Prefabs.MagnifierGlass!;

    public override string HelpText => "Destroys all bugs in target tile and all tiles around it.";

    protected override Task Use(Board board, Tile target)
    {
        foreach (var coordinate in target.Coord.Yield().Concat(target.Coord.EnumerateAdjacent()))
        {
            board[coordinate].Piece?.Destroy();
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
