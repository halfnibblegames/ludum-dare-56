using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects.Pieces;

namespace HalfNibbleGame.Objects.Cards;

public sealed class Feature : CardWithTarget
{
    private readonly Random random = new Random();
    public Feature() : base("It's a feature")
    {
    }

    protected override async Task Use(Board board, Tile target)
    {
        // Destroy what's in the tile.
        var possiblePieces = new List<PackedScene>()
        {
            Global.Prefabs.Ant!,
            Global.Prefabs.Dragonfly!,
            Global.Prefabs.Grasshopper!,
            Global.Prefabs.HornedBeetle!,
            Global.Prefabs.PrayingMantis!
        };

        var isEnemy = target.Piece!.IsEnemy;
        possiblePieces.RemoveAt(
            target.Piece switch
            {
                Ant => 0,
                Dragonfly => 1,
                Grasshopper => 2,
                HornedBeetle => 3,
                _ => 4
            }
        );

        target.Piece?.Destroy();

        // A new piece takes its place.
        var randomPiece = possiblePieces[random.Next(possiblePieces.Count)].Instance<Piece>();
        randomPiece.IsEnemy = isEnemy;
        await board.AddPiece(randomPiece, target.Coord);
    }

    public override Texture GetTexture() => Global.Prefabs.Feature!;
}
