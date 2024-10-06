using System;
using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;
using Object = Godot.Object;

namespace HalfNibbleGame.Objects;

public sealed class Tile : Area2D
{
    private readonly Vector2 origin = 0.5f * size + (Board.Height - 1) * size.y * Vector2.Down;

    private const float width = 16;
    private const float height = 16;
    private static readonly Vector2 size = new(width, height);
    private static readonly char[] rows = { '1', '2', '3', '4', '5', '6', '7', '8' };
    private static readonly char[] cols = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

    private TileCoord coord;
    public TileCoord Coord
    {
        get => coord;
        set
        {
            coord = value;
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        Position = origin + new Vector2(Coord.X * size.x, -Coord.Y * size.y + HeightOffsetInPixels);
    }

    private float heightOffsetInPixels;
    public float HeightOffsetInPixels
    {
        get => heightOffsetInPixels;
        set
        {
            heightOffsetInPixels = value;
            UpdatePosition();
        }
    }

    private bool isHovered;
    private TileAction actionShown;
    private readonly Dictionary<TileAction, Sprite> actionSprites = new();

    private TileColor color;
    public TileColor Color
    {
        get => color;
        set
        {
            color = value;
            updateAnimation();
        }
    }

    private Piece? piece;
    public Piece? Piece
    {
        get => piece;
        set
        {
            if (piece == value) return;
            if (!isHovered)
            {
                piece = value;
                return;
            }
            piece?.EndHover();
            piece = value;
            piece?.StartHover();
        }
    }

    public override void _Ready()
    {
        actionSprites.Add(TileAction.Swipe, GetNode<Sprite>("HighlightSprites/SwipeTile"));
        actionSprites.Add(TileAction.Capture, GetNode<Sprite>("HighlightSprites/AttackTile"));
        actionSprites.Add(TileAction.Move, GetNode<Sprite>("HighlightSprites/SelectableTile"));

        updateAnimation();
    }

    public override void _InputEvent(Object viewport, InputEvent @event, int shapeIdx)
    {
        var cursor = Global.Services.Get<Cursor>();

        if (@event is InputEventMouseMotion)
        {
            cursor.MoveToTile(this);
            if (piece is not null)
            {
                Global.Services.Get<HelpService>().ShowHelp(piece);
            }
            else
            {
                Global.Services.Get<HelpService>().ClearHelp();
            }
        }

        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: 1 })
        {
            cursor.Confirm();
        }
    }

    public override string ToString() => $"{cols[Coord.X]}{rows[Coord.Y]}";

    public void ShowAction(TileAction action)
    {
        actionShown = action;
        updateAnimation();
    }

    public void ResetAction()
    {
        actionShown = TileAction.None;
        updateAnimation();
    }

    public void StartHover()
    {
        isHovered = true;
        Piece?.StartHover();
    }

    public void EndHover()
    {
        Piece?.EndHover();
        isHovered = false;
    }

    private void updateAnimation()
    {
        GetNode<AnimatedSprite>("AnimatedSprite").Animation = chooseAnimation();
        foreach (var sprite in actionSprites.Values)
        {
            sprite.Visible = false;
        }
        if (actionSprites.TryGetValue(actionShown, out var visibleSprite))
        {
            GD.Print($"Showing {visibleSprite.Name}");
            visibleSprite.Visible = true;
        }
    }

    private string chooseAnimation()
    {
        return (Color) switch
        {
            (TileColor.Light) => "Light",
            (TileColor.Dark) => "Dark",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum TileColor : byte
{
    Light = 0,
    Dark = 1
}

public enum TileAction : byte
{
    None = 0,
    Move = 1,
    Capture = 2,
    Swipe = 3,
}
