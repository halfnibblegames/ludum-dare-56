using Godot;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class MoveAnimation : Node
{
    [Signal] public delegate void Finished();

    private const float duration = 0.2f;
    private const float jumpHeight = 4;

    private readonly Vector2 startPosition;
    private readonly Vector2 endPosition;
    private readonly Piece target;

    private Vector2 difference => endPosition - startPosition;
    private float t => Mathf.Clamp(passedTime / duration, 0, 1);

    private float passedTime;

    public MoveAnimation(Vector2 startPosition, Vector2 endPosition, Piece target)
    {
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.target = target;
    }

    public override void _Process(float delta)
    {
        if (!IsInstanceValid(target))
        {
            return;
        }

        passedTime += delta;
        target.Position = linearComponent + jumpComponent;

        if (passedTime >= duration)
        {
            EmitSignal(nameof(Finished));
            QueueFree();
        }
    }

    private Vector2 linearComponent => startPosition + Easing.InOutQuad(t) * difference;
    private Vector2 jumpComponent => Vector2.Up * jumpHeight * Mathf.Sin(t * Mathf.Pi);
}
