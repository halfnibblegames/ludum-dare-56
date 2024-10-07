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
            army(
                enemy(Prefabs.Grasshopper, 3, 7),
                enemy(Prefabs.Grasshopper, 4, 7),
                enemy(Prefabs.Ant, 2, 7),
                enemy(Prefabs.Ant, 5, 7)
            )
        ),
        new(
            2,
            army(
                enemy(Prefabs.Dragonfly, 3, 7),
                enemy(Prefabs.Dragonfly, 4, 7),
                enemy(Prefabs.Grasshopper, 2, 7),
                enemy(Prefabs.Grasshopper, 5, 7),
                enemy(Prefabs.Ant, 3, 6),
                enemy(Prefabs.Ant, 4, 6)
            )
        ),
        new(
            3,
            army(
                enemy(Prefabs.HornedBeetle, 2, 7),
                enemy(Prefabs.HornedBeetle, 5, 7),
                enemy(Prefabs.Ant, 1, 7),
                enemy(Prefabs.Ant, 3, 7),
                enemy(Prefabs.Ant, 4, 7),
                enemy(Prefabs.Ant, 6, 7),
                enemy(Prefabs.Ant, 2, 6),
                enemy(Prefabs.Ant, 5, 6)
            )
        )
    };

    public IReadOnlyList<PackedScene> InitialArmy { get; } = army(
        Prefabs.QueenBee!,
        Prefabs.PrayingMantis!,
        Prefabs.Ant!,
        Prefabs.Ant!
    );

    private static IReadOnlyList<PackedScene> army(params PackedScene[] units) => units;
    private static IReadOnlyList<Enemy> army(params Enemy[] units) => units;
    private static Enemy enemy(PackedScene? unit, int x, int y) => new(unit!, new TileCoord(x, y));

    public sealed record Level(int Number, IReadOnlyList<Enemy> EnemyForce);
    public sealed record Enemy(PackedScene Prefab, TileCoord Location);
}
