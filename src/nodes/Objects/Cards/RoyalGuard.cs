using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects.Pieces;

namespace HalfNibbleGame.Objects.Cards;

public sealed class RoyalGuard : Card
{
    public RoyalGuard() : base("Royal Guard")
    {
    }
    
    public override Texture GetTexture() => Global.Prefabs.RoyalGuard!;

    public override async Task Use(Board board)
    {
        var queenBeeTile = board.Tiles.Single(t => t.Piece is QueenBee);
        if (queenBeeTile is null)
        {
            throw new InvalidOperationException("How are you playing without a queen bee?");
        }

        foreach (var adjacentTile in queenBeeTile.Coord.EnumerateOrthogonal())
        {
            if (board[adjacentTile].Piece is not null)
                continue;

            var ant = Global.Prefabs.Ant!.Instance<Piece>();
            await board.AddPiece(ant, adjacentTile);
        }
    }
    public override string HelpText => "Spawns ants in all free tiles orthogonal to the queen.\n[b]- For the queen![/b]";
}
