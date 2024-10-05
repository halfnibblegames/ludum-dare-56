﻿using Godot;

namespace HalfNibbleGame.Objects;

public sealed class Tile : Area2D
{
    public delegate void TileClickedEventHandler();

    private const float width = 16;
    private const float height = 16;
    public static readonly Vector2 Size = new(width, height);
    private static readonly char[] rows = { '1', '2', '3', '4', '5', '6', '7', '8' };
    private static readonly char[] cols = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

    public TileCoord Coord;
    public int Col => Coord.X;
    public int Row => Coord.Y;

    private TileColor color;
    public TileColor Color
    {
        get => color;
        set
        {
            color = value;
            applyColor();
        }
    }

    public Piece? Piece;

    public event TileClickedEventHandler? Clicked;

    public override void _Ready()
    {
        applyColor();
    }

    private void applyColor()
    {
        GetNode<AnimatedSprite>("AnimatedSprite").Animation = color == TileColor.Dark ? "Dark" : "Light";
    }

    public override void _InputEvent(Object viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton { Pressed: true })
        {
            Clicked?.Invoke();
        }
    }

    public override string ToString() => $"{cols[Col]}{rows[Row]}";
}

public enum TileColor : byte
{
    Light = 0,
    Dark = 1
}
