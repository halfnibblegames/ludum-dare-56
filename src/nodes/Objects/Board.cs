﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

public sealed class Board : Node2D
{
    public const int Width = 8;
    public const int Height = 8;

    private Tile[] tiles = Array.Empty<Tile>();
    private readonly List<Piece> pieces = new();

    public IReadOnlyList<Piece> Pieces { get; }
    public IReadOnlyList<Tile> Tiles => tiles;

    public Board()
    {
        Pieces = pieces.AsReadOnly();
    }

    public Move PreviewMove(Piece piece, Tile from, Tile to, int previousMovesInTurn)
    {
        return new Move(this, piece, from, to, previousMovesInTurn);
    }

    public override void _Ready()
    {
        GetNode("EditorRect").QueueFree();
    }

    public async Task Disappear()
    {
        await Task.WhenAll(pieces.Select(p => fadePiece(p, 0, FadeCurve.FadeOut)));
        await fadeBoard(FadeCurve.FadeOut);
    }

    public async Task Reset()
    {
        cleanUp();
        await setUp();
    }

    private void cleanUp()
    {
        foreach (var p in pieces)
        {
            p.QueueFree();
        }
        pieces.Clear();
        foreach (var t in tiles)
        {
            t.QueueFree();
        }
        tiles = Array.Empty<Tile>();
    }

    private async Task setUp()
    {
        // Make the origin be bottom left.
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

                tile.Color = (TileColor) ((x + y) % 2);
                tile.Coord = new TileCoord(x, y);
                tiles[toIndex(x, y)] = tile;
            }
        }

        await fadeBoard(FadeCurve.FadeIn);
    }

    private async Task fadeBoard(FadeCurve fadeCurve)
    {
        var anim = new BoardRippleAnimation(this, Enumerable.Range(0, 8).Select(x => new TileCoord(x, 0)).ToList(), 7, fadeCurve);
        var signal = ToSignal(anim, nameof(BoardRippleAnimation.Finished));
        AddChild(anim);
        await signal;
    }

    public async Task AddPiece(Piece piece, TileCoord tileCoord)
    {
        pieces.Add(piece);
        AddChild(piece);
        var tile = tiles[toIndex(tileCoord)];
        piece.Position = tile.Position;
        tile.Piece = piece;
        piece.Destroyed += () => pieces.Remove(piece);
        await fadeInPiece(piece,
            Math.Min(tileCoord.Manhattan(new TileCoord(3, 0)), tileCoord.Manhattan(new TileCoord(3, 7))));
    }

    private async Task fadeInPiece(Piece piece, int distance)
    {
        await fadePiece(piece, distance, FadeCurve.FadeIn);
    }

    private async Task fadePiece(Piece piece, int distance, FadeCurve curve)
    {
        const int delayPerDistance = 100;

        piece.Modulate = new Color(1, 1, 1, 0);
        await Task.Delay(delayPerDistance * distance);
        var anim = new FadeAnimation(curve, piece);
        var signal = ToSignal(anim, nameof(FadeAnimation.Finished));
        AddChild(anim);
        await signal;
    }

    public void ResetHighlightedTiles()
    {
        foreach (var t in tiles)
        {
            t.ResetAction();
        }
    }

    public Tile this[TileCoord coord] => tiles[toIndex(coord)];

    private static int toIndex(TileCoord coord) => toIndex(coord.X, coord.Y);
    private static int toIndex(int x, int y) => y * Width + x;
}
