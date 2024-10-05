using Godot;
using JetBrains.Annotations;

namespace HalfNibbleGame.Objects;

public sealed class Tile : Area2D
{
    private readonly Vector2 origin = 0.5f * size + (Board.Height - 1) * size.y * Vector2.Down;
    public delegate void TileClickedEventHandler();

    private bool beingHovered = false;
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

    public int Col => Coord.X;
    public int Row => Coord.Y;

    private bool isHighlighted;

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

    public Piece? Piece;
    private AnimatedSprite cursor = default!;

    public event TileClickedEventHandler? Clicked;

    public override void _Ready()
    {
        updateAnimation();
        cursor = GetParent<Board>().GetNode<AnimatedSprite>("Cursor");
    }

    public override void _InputEvent(Object viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseMotion)
        {
            cursor.Position = Position;
            cursor.Visible = true;
        }

        if (@event is InputEventMouseButton { Pressed: true })
        {
            cursor.Animation = "Confirm";
            Clicked?.Invoke();
        }
    }

    public override string ToString() => $"{cols[Col]}{rows[Row]}";

    public void Highlight()
    {
        isHighlighted = true;
        updateAnimation();
    }

    public void ResetHighlight()
    {
        isHighlighted = false;
        updateAnimation();
    }

    private void updateAnimation()
    {
        GetNode<AnimatedSprite>("AnimatedSprite").Animation = chooseAnimation();
    }

    private string chooseAnimation()
    {
        return (color, isHighlighted) switch
        {
            (TileColor.Light, false) => "Light",
            (TileColor.Light, true) => "SelectedLight",
            (TileColor.Dark, false) => "Dark",
            (TileColor.Dark, true) => "SelectedDark",
        };
    }
}

public enum TileColor : byte
{
    Light = 0,
    Dark = 1
}
