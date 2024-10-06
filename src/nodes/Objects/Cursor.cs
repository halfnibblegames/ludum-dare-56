using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

sealed class Cursor : AnimatedSprite
{
    public delegate void TileEventHandler(Board board, Tile tile);

    private const float transitionDuration = 0.05f;

    private Board board = null!;

    private Vector2 startPos;
    private float timeSinceStart;
    private Tile? targetTile;

    public event TileEventHandler? TileClicked;
    public event TileEventHandler? TileHovered;

    public override void _Ready()
    {
        Global.Services.ProvideInScene(this);
        Play("Highlight");
        board = GetParent<Board>();
        Visible = false;
    }

    public void Activate()
    {
        if (targetTile != null)
        {
            startPos = targetTile.Position;
            Visible = true;
        }
    }

    public void Deactivate()
    {
        Visible = false;
    }

    public void Reset()
    {
        Visible = false;
        targetTile = null;
    }

    public void MoveToTile(Tile tile)
    {
        if (tile == targetTile) return;

        startPos = targetTile?.Position ?? Position;
        Visible = true;

        targetTile = tile;
        timeSinceStart = 0;
        TileHovered?.Invoke(board, tile);
    }

    public void Confirm()
    {
        if (targetTile is null) return;

        Play("Confirm");
        TileClicked?.Invoke(board, targetTile);

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
