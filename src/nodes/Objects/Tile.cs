using Godot;

namespace HalfNibbleGame.Objects;

public sealed class Tile : Node2D
{
    private const float width = 16;
    private const float height = 16;
    public static readonly Vector2 Size = new(width, height);

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

    public override void _Ready()
    {
        applyColor();
    }

    private void applyColor()
    {
        GetNode<AnimatedSprite>("AnimatedSprite").FlipH = color == TileColor.Dark;
    }
}

public enum TileColor : byte
{
    Light = 0,
    Dark = 1
}
