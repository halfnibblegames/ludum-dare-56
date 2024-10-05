using System;
using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

public sealed class Board : Node2D
{
    public const int Width = 8;
    public const int Height = 8;

    private readonly InputHandler input;
    private Tile[] tiles = Array.Empty<Tile>();
    private readonly List<Piece> pieces = new();

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

        // Spawn hardcoded pieces for testing
        spawnObjectDebug(Global.Prefabs.QueenBee, 2, 2);
        spawnObjectDebug(Global.Prefabs.QueenBee, 6, 6);
        spawnObjectDebug(Global.Prefabs.HornedBeetle, 1, 6);
        spawnObjectDebug(Global.Prefabs.PrayingMantis, 3, 6);
        spawnObjectDebug(Global.Prefabs.HornedBeetle, 4, 6, true);
        spawnObjectDebug(Global.Prefabs.PrayingMantis, 6, 1, true);
    }

    private void spawnObjectDebug(PackedScene? prefab, int x, int y, bool isEnemy = false)
    {
        var piece = prefab?.InstanceOrNull<Piece>() ?? throw new Exception("Could not instantiate piece.");
        piece.IsEnemy = isEnemy;
        AddPiece(piece, new TileCoord(x, y));
    }

    public void AddPiece(Piece piece, TileCoord tileCoord)
    {
        pieces.Add(piece);
        AddChild(piece);
        var queenTile = tiles[toIndex(tileCoord)];
        piece.Position = queenTile.Position;
        queenTile.Piece = piece;
    }

    public void ResetHighlightedTiles()
    {
        foreach (var t in tiles)
        {
            t.ResetHighlight();
        }
    }

    public Tile this[TileCoord coord] => tiles[toIndex(coord)];

    private static int toIndex(TileCoord coord) => toIndex(coord.X, coord.Y);
    private static int toIndex(int x, int y) => y * Width + x;
    private static TileCoord toCoord(int i) => new(i / Width, i % Width);
}
