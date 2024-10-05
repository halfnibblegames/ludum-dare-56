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
        // Make the origin be bottom left.
        var origin = 0.5f * Tile.Size + (height - 1) * Tile.Size.y * Vector2.Down;
        tiles = new Tile[width * height];
        var prefab = Global.Prefabs.Tile ?? throw new Exception("Could not find tile instance.");

        // Create the board back to front to get the right draw order
        for (var y = height - 1; y >= 0; y--)
        {
            for (var x = width - 1; x >= 0; x--)
            {
                if (prefab.Instance() is not Tile tile)
                {
                    throw new Exception("Instantiated tile was not a tile.");
                }

                AddChild(tile);

                var pos = origin + new Vector2(x * Tile.Size.x, -y * Tile.Size.y);
                tile.Position = pos;
                tile.Color = (TileColor) ((x + y) % 2);
                tile.Col = x;
                tile.Row = y;

                tiles[toIndex(x, y)] = tile;
            }
        }
    }

    private static int toIndex(int x, int y) => y * width + x;
    private static (int X, int Y) fromIndex(int i) => (i / width, i % width);
}
