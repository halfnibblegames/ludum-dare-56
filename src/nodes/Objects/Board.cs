using System;
using Godot;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame.Objects;

[Tool]
public sealed class Board : Node2D
{
    private const int width = 8;
    private const int height = 8;

    private Tile[] tiles = Array.Empty<Tile>();

    public override void _Ready()
    {
        var origin = 0.5f * Tile.Size;
        tiles = new Tile[width * height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pos = origin + new Vector2(x * Tile.Size.x, y + Tile.Size.y);
                var tile = Global.Prefabs.Tile!.Instance<Tile>();
                AddChild(tile);
                tile.Position = pos;
                tiles[toIndex(x, y)] = tile;
            }
        }
    }

    private static int toIndex(int x, int y) => y * width + x;
    private static (int X, int Y) fromIndex(int i) => (i / width, i % width);
}
