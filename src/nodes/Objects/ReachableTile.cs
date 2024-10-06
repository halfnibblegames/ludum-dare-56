namespace HalfNibbleGame.Objects;

public sealed record ReachableTile(TileCoord Coord, TileAction Action);

public static class ReachableTileExtensions
{
    public static ReachableTile MoveTo(this TileCoord coord) => new(coord, TileAction.Move);
    public static ReachableTile Capture(this TileCoord coord) => new(coord, TileAction.Capture);
    public static ReachableTile Swipe(this TileCoord coord) => new(coord, TileAction.Swipe);
}
