using System.Collections.Generic;
using Godot;

using static HalfNibbleGame.Autoload.Global;

namespace HalfNibbleGame.Systems;

public sealed class Levels
{
    public Level[] All { get; } =
    {
        new(
            1,
            pieceChoice(null, null),
            army(
                enemy(Prefabs.PrayingMantis, 3, 7),
                enemy(Prefabs.PrayingMantis, 4, 7),
                enemy(Prefabs.Ant, 2, 7),
                enemy(Prefabs.Ant, 5, 7)
            )),
        new(
            2,
            pieceChoice(Prefabs.Grasshopper, Prefabs.Dragonfly),
            army(
                enemy(Prefabs.Dragonfly, 3, 7),
                enemy(Prefabs.Dragonfly, 4, 7),
                enemy(Prefabs.PrayingMantis, 2, 7),
                enemy(Prefabs.PrayingMantis, 5, 7),
                enemy(Prefabs.Ant, 3, 6),
                enemy(Prefabs.Ant, 4, 6)
            )),
        new(
            3,
            pieceChoice(Prefabs.Dragonfly, Prefabs.PrayingMantis),
            army(
                enemy(Prefabs.HornedBeetle, 2, 7),
                enemy(Prefabs.HornedBeetle, 5, 7),
                enemy(Prefabs.Ant, 1, 7),
                enemy(Prefabs.Ant, 3, 7),
                enemy(Prefabs.Ant, 4, 7),
                enemy(Prefabs.Ant, 6, 7),
                enemy(Prefabs.Ant, 2, 6),
                enemy(Prefabs.Ant, 5, 6)
            )),
        new(
            4,
            pieceChoice(Prefabs.HornedBeetle, Prefabs.Grasshopper),
            army(
                enemy(Prefabs.Grasshopper, 2, 6),
                enemy(Prefabs.Grasshopper, 5, 6),
                enemy(Prefabs.Dragonfly, 2, 7),
                enemy(Prefabs.Dragonfly, 5, 7),
                enemy(Prefabs.PrayingMantis, 3, 7),
                enemy(Prefabs.PrayingMantis, 4, 7)
            )),
        new(
            5,
            pieceChoice(Prefabs.Grasshopper, Prefabs.Dragonfly),
            army(
                enemy(Prefabs.HornedBeetle, 1, 7),
                enemy(Prefabs.HornedBeetle, 3, 7),
                enemy(Prefabs.HornedBeetle, 5, 7),
                enemy(Prefabs.Dragonfly, 2, 7),
                enemy(Prefabs.Dragonfly, 4, 7),
                enemy(Prefabs.Dragonfly, 6, 7),
                enemy(Prefabs.PrayingMantis, 1, 6),
                enemy(Prefabs.PrayingMantis, 3, 6),
                enemy(Prefabs.PrayingMantis, 5, 6)
            )),
        new(
            6,
            pieceChoice(Prefabs.HornedBeetle, Prefabs.PrayingMantis),
            army(
                enemy(Prefabs.HornedBeetle, 0, 7),
                enemy(Prefabs.HornedBeetle, 7, 7),
                enemy(Prefabs.Grasshopper, 1, 7),
                enemy(Prefabs.Grasshopper, 6, 7),
                enemy(Prefabs.Dragonfly, 2, 7),
                enemy(Prefabs.Dragonfly, 5, 7),
                enemy(Prefabs.PrayingMantis, 3, 7),
                enemy(Prefabs.PrayingMantis, 4, 7),
                enemy(Prefabs.Ant, 0, 6),
                enemy(Prefabs.Ant, 1, 6),
                enemy(Prefabs.Ant, 2, 6),
                enemy(Prefabs.Ant, 3, 6),
                enemy(Prefabs.Ant, 4, 6),
                enemy(Prefabs.Ant, 5, 6),
                enemy(Prefabs.Ant, 6, 6),
                enemy(Prefabs.Ant, 7, 6)
            )),
    };

    public IReadOnlyList<PackedScene> InitialArmy { get; } = army(
        Prefabs.QueenBee!,
        Prefabs.Dragonfly!,
        Prefabs.Ant!,
        Prefabs.Ant!
    );

    private static IReadOnlyList<PackedScene> army(params PackedScene[] units) => units;
    private static IReadOnlyList<Enemy> army(params Enemy[] units) => units;
    private static Enemy enemy(PackedScene? unit, int x, int y) => new(unit!, new TileCoord(x, y));
    private static IReadOnlyList<PackedScene> pieceChoice(PackedScene? left, PackedScene? right) =>
        new[] { left!, right! };

    public sealed record Level(int Number, IReadOnlyList<PackedScene> PieceChoices, IReadOnlyList<Enemy> EnemyForce);
    public sealed record Enemy(PackedScene Prefab, TileCoord Location);
}
