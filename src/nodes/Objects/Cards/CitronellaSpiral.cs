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

    public override string HelpText => "Transforms target piece into an ant.\n[b]How do these things even work?[/b]";

    protected override IEnumerable<ReachableTile> GetReachableTiles(Board board)
    {
        return board.Tiles.Where(t => t.Piece is { IsEnemy: true })
            .Select(t => t.Coord.MoveTo());
    }

    protected override async Task Use(Board board, Tile target)
    {
        var isEnemy = target.Piece!.IsEnemy;
        target.Piece?.Destroy();
        target.Piece = null;

        var randomPiece = Global.Prefabs.Ant!.Instance<Piece>();
        randomPiece.IsEnemy = isEnemy;
        await board.AddPiece(randomPiece, target.Coord);
    }
}
