using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class BoardRippleAnimation : Node
{
    [Signal] public delegate void Finished();

    private const float duration = 0.2f;
    private const float tileDisplacementInPixels = 4f;

    private readonly IEnumerable<IGrouping<int, TileCoord>> tilesToAnimate;

    private float t => Mathf.Clamp(passedTime / duration, 0, 1);

    private float passedTime;
    private readonly Board board;

    public BoardRippleAnimation(Board board, Tile origin, int radius)
    {
        if (radius < 0)
            throw new InvalidOperationException("Radius must be at least 0 for self");

        this.board = board;
        tilesToAnimate =
            Enumerable.Range(0, radius)
                .SelectMany(distance => distance == 0 ? new[] { origin.Coord } : origin.Coord.EnumerateValidNeighboringAtRadius(distance))
                .GroupBy(x => origin.Coord.DiagonalDistance(x));
    }

    public override void _Process(float delta)
    {
        passedTime += delta;

        foreach (var groupOfTiles in tilesToAnimate)
        {
            var distanceFromOrigin = groupOfTiles.Key;
            foreach (var tileCoordinate in groupOfTiles)
            {
                var tile = board[tileCoordinate];
                tile.HeightOffsetInPixels = tileDisplacementInPixels * Mathf.Sin(t * distanceFromOrigin * Mathf.Pi);
            }
        }

        if (passedTime >= duration)
        {
            EmitSignal(nameof(Finished));
            QueueFree();
        }
    }
}
