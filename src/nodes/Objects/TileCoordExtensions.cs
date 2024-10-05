using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects;

public static class TileCoordExtensions
{
    private static readonly Step[] ccwOrthogonalSteps =
        { Step.Right, Step.Up, Step.Left, Step.Down };
    private static readonly Step[] ccwDiagonalSteps =
        { Step.UpRight, Step.UpLeft, Step.DownLeft, Step.DownRight };
    private static readonly Step[] ccwSteps =
        { Step.Right, Step.UpRight, Step.Up, Step.UpLeft, Step.Left, Step.DownLeft, Step.Down, Step.DownRight };

    public static IEnumerable<TileCoord> WhereValid(this IEnumerable<TileCoord> coords) =>
        coords.Where(c => c.IsValid());

    public static IEnumerable<TileCoord> EnumerateOrthogonal(this TileCoord c) =>
        ccwOrthogonalSteps.Select(s => c + s).WhereValid();

    public static IEnumerable<TileCoord> EnumerateDiagonal(this TileCoord c) =>
        ccwDiagonalSteps.Select(s => c + s).WhereValid();

    public static IEnumerable<TileCoord> EnumerateAdjacent(this TileCoord c) =>
        c.EnumerateAtRadius(1);

    public static IEnumerable<TileCoord> EnumerateAtRadius(this TileCoord c, int distanceFromCenter)
    {
        // Go south-west to the correct starting point.
        var current = c;
        for (var i = 0; i < distanceFromCenter; i++)
        {
            current += Step.DownLeft;
        }

        // For each of the base directions
        foreach (var step in ccwOrthogonalSteps)
        {
            // Go in that direction 2 * distanceFromCenter times
            for (var i = 0; i < distanceFromCenter * 2; i++)
            {
                current += step;
                yield return current;
            }
        }
    }

    public static IEnumerable<TileCoord> EnumerateDirection(this TileCoord c, Step step)
    {
        var t = c + step;
        while (t.IsValid())
        {
            yield return t;
            t += step;
        }
    }
}
