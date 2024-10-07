using System.Collections.Generic;
using Godot;

using static HalfNibbleGame.Autoload.Global;

namespace HalfNibbleGame.Systems;

sealed class Levels
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
            ))
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
