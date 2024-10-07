using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects.Pieces;

namespace HalfNibbleGame.Objects.Cards;

public sealed class Hivemind : Card
{
    private static readonly Random random = new Random();

    public Hivemind() : base("Hivemind")
    {
    }

    public override Texture GetTexture() => Global.Prefabs.Hivemind!;

    public override async Task<bool> Use(Board board)
    {
        var queenBeeTile = board.Tiles.Single(t => t.Piece is QueenBee);
        if (queenBeeTile is null)
        {
            throw new InvalidOperationException("How are you playing without a queen bee?");
        }

        var closestEmptyTiles = queenBeeTile.Coord.EnumerateOrthogonal()
            .Concat(queenBeeTile.Coord.EnumerateOrthogonal(2))
            .Concat(board.Tiles.Select(x => x.Coord));

        foreach (var adjacentTile in closestEmptyTiles)
        {
            if (board[adjacentTile].Piece is not null)
                continue;
            var prefab = (random.NextDouble() >= 0.5 ? Global.Prefabs.PrayingMantis : Global.Prefabs.Grasshopper);
            await board.AddPiece(prefab!.Instance<Piece>(), adjacentTile);
        }

        return true;
    }
    
    public override string HelpText => "Summons a Grasshopper or a Praying Mantis near the Queen.\n[b]- For the Queen![/b]";
}
