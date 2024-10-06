using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

sealed class Cursor : AnimatedSprite
{
    private const float transitionDuration = 0.05f;

    private Vector2 startPos;
    private float timeSinceStart;
    private Tile? targetTile;

    public override void _Ready()
    {
        Play("Highlight");
    }

    public void MoveToTile(Tile tile)
    {
        if (tile == targetTile) return;

        Visible = true;

        if (targetTile != null)
        {
            startPos = targetTile.Position;
            targetTile.EndHover();
        }
        else
        {
            startPos = Position;
        }

        targetTile = tile;
        timeSinceStart = 0;
        tile.StartHover();
    }

    public void Confirm()
    {
        Play("Confirm");

        if (targetTile?.Piece is { IsEnemy: false })
        {
            Global.Services.Get<Boombox>().Play(Boombox.SoundEffect.Select);
        }
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
