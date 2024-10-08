﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects;

public static class TileCoordExtensions
{
    private static readonly Step[] ccwOrthogonalSteps =
        { Step.Right, Step.Up, Step.Left, Step.Down };
    public static readonly Step[] DiagonalSteps =
        { Step.UpRight, Step.UpLeft, Step.DownLeft, Step.DownRight };
    public static readonly Step[] Steps =
        { Step.Right, Step.UpRight, Step.Up, Step.UpLeft, Step.Left, Step.DownLeft, Step.Down, Step.DownRight };

    public static IEnumerable<TileCoord> WhereValid(this IEnumerable<TileCoord> coords) =>
        coords.Where(c => c.IsValid());

    public static IEnumerable<TileCoord> EnumerateOrthogonal(this TileCoord c, int distance = 1) =>
        ccwOrthogonalSteps.Select(s => c + s * distance).WhereValid();

    public static IEnumerable<TileCoord> EnumerateDiagonal(this TileCoord c, int distance = 1) =>
        DiagonalSteps.Select(s => c + s * distance).WhereValid();

    public static IEnumerable<TileCoord> EnumerateAdjacent(this TileCoord c) =>
        Steps.Select(s => c + s).WhereValid();

    public static IEnumerable<TileCoord> EnumerateDirection(this TileCoord c, Step step)
    {
        if (step is { X: 0, Y: 0 }) throw new ArgumentOutOfRangeException(nameof(step));
        var t = c + step;
        while (t.IsValid())
        {
            yield return t;
            t += step;
        }
    }

    public static IEnumerable<TileCoord> EnumerateKnightMoves(this TileCoord c)
    {
        for (var i = 0; i < ccwOrthogonalSteps.Length; i++)
        {
            var step1 = ccwOrthogonalSteps[i];
            var step2 = ccwOrthogonalSteps[(i + 1) % ccwOrthogonalSteps.Length];

            var t1 = c + step1 + step1 + step2;
            if (t1.IsValid()) yield return t1;
            var t2 = c + step1 + step2 + step2;
            if (t2.IsValid()) yield return t2;
        }
    }
}
