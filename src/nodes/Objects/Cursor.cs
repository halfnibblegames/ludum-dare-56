using Godot;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

sealed class Cursor : AnimatedSprite
{
    private const float transitionDuration = 0.05f;

    private Vector2 startPos;
    private float timeSinceStart;
    private Tile? targetTile;

    public void MoveToTile(Tile tile)
    {
        if (tile == targetTile) return;

        Visible = true;

        if (targetTile is null)
        {
            targetTile = tile;
            Position = tile.Position;
            return;
        }

        startPos = Position;
        timeSinceStart = 0;
        targetTile = tile;
    }

    public void Confirm()
    {
        Play("Confirm");
    }

    public override void _Process(float delta)
    {
        if (targetTile is null) return;

        var remainingDiff = targetTile.Position - Position;
        if (remainingDiff.LengthSquared() < 0.01f)
        {
            Position = targetTile.Position;
            return;
        }

        timeSinceStart += delta;

        var t = Easing.OutCubic(Mathf.Clamp(timeSinceStart / transitionDuration, 0f, 1f));
        var diff = targetTile.Position - startPos;
        Position = startPos + diff * t;
    }

    public void OnAnimationFinished()
    {
        if (Animation != "Highlight")
        {
            Play("Highlight");
        }
    }
}
