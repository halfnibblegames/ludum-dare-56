using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class BoardRippleAnimation : Node
{
    [Signal] public delegate void Finished();

    private const float rippleDuration = 7 * distanceOffset;
    private const float distanceOffset = 0.1f;
    private const float tileDisplacementInPixels = 4f;

    private readonly Dictionary<TileCoord, int> tilesWithDistance = new();

    private float passedTime;
    private readonly Board board;
    private readonly float duration;

    public BoardRippleAnimation(Board board, IEnumerable<TileCoord> startTiles, int distance)
    {
        if (distance < 0)
            throw new InvalidOperationException("Distance must be at least 0 for self");

        this.board = board;
        duration = rippleDuration + distance * distanceOffset;

        var q = new Queue<TileCoord>();
        foreach (var startTile in startTiles)
        {
            q.Enqueue(startTile);
            tilesWithDistance.Add(startTile, 0);
        }

        while (q.Count > 0)
        {
            var tile = q.Dequeue();
            var d = tilesWithDistance[tile];
            if (d == distance) continue;
            foreach (var nextTile in tile.EnumerateAdjacent().Where(t => !tilesWithDistance.ContainsKey(t)))
            {
                tilesWithDistance.Add(nextTile, d + 1);
                q.Enqueue(nextTile);
            }
        }
    }

    public override void _Process(float delta)
    {
        passedTime += delta;

        foreach (var kvp in tilesWithDistance)
        {
            var tile = board[kvp.Key];
            var distance = kvp.Value;
            var startTime = distance * distanceOffset;
            var timeSinceStart = passedTime - startTime;
            var t = Mathf.Clamp(timeSinceStart / rippleDuration, 0f, 1f);
            tile.HeightOffsetInPixels = tileDisplacementInPixels * Mathf.Sin(t * Mathf.Pi);
        }

        if (passedTime >= duration)
        {
            EmitSignal(nameof(Finished));
            QueueFree();
        }
    }
}
