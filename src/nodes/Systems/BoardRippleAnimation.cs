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

    private readonly Dictionary<TileCoord, int> tilesWithDistance = new();

    private float t => Mathf.Clamp(passedTime / duration, 0, 1);

    private float passedTime;
    private readonly Board board;

    public BoardRippleAnimation(Board board, TileCoord origin, int radius)
        : this(board, new[] { origin }, radius) { }

    public BoardRippleAnimation(Board board, IEnumerable<TileCoord> startTiles, int distance)
    {
        if (distance < 0)
            throw new InvalidOperationException("Distance must be at least 0 for self");

        this.board = board;

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
            }
        }
    }

    public override void _Process(float delta)
    {
        passedTime += delta;

        foreach (var kvp in tilesWithDistance)
        {
            var tile = board[kvp.Key];
            tile.HeightOffsetInPixels = tileDisplacementInPixels * Mathf.Sin(t * kvp.Value * Mathf.Pi);
        }

        if (passedTime >= duration)
        {
            EmitSignal(nameof(Finished));
            QueueFree();
        }
    }
}
