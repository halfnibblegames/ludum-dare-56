using System;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

[Tool]
public sealed class Board : Node2D
{
    public const int Width = 8;
    public const int Height = 8;

    private readonly InputHandler input;
    private Tile[] tiles = Array.Empty<Tile>();

    public Board()
    {
        input = new InputHandler(this);
    }

    public override void _Ready()
    {
        // Make the origin be bottom left.
        var origin = 0.5f * Tile.Size + (Height - 1) * Tile.Size.y * Vector2.Down;
        tiles = new Tile[Width * Height];
        var prefab = Global.Prefabs.Tile ?? throw new Exception("Could not find tile instance.");

        // Create the board back to front to get the right draw order
        for (var y = Height - 1; y >= 0; y--)
        {
            for (var x = Width - 1; x >= 0; x--)
            {
                if (prefab.Instance() is not Tile tile)
                {
                    throw new Exception("Instantiated tile was not a tile.");
                }

                AddChild(tile);

                var pos = origin + new Vector2(x * Tile.Size.x, -y * Tile.Size.y);
                tile.Position = pos;
                tile.Color = (TileColor) ((x + y) % 2);
                tile.Coord = new TileCoord(x, y);

                tiles[toIndex(x, y)] = tile;

                tile.Clicked += () => input.HandleTileClick(tile);
            }
        }
    }

    private static int toIndex(TileCoord coord) => toIndex(coord.X, coord.Y);
    private static int toIndex(int x, int y) => y * Width + x;
    private static TileCoord toCoord(int i) => new(i / Width, i % Width);
}
