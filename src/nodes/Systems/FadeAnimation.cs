using Godot;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

sealed class FadeAnimation : Node
{
    [Signal] public delegate void Finished();

    private const float duration = 0.3f;

    private readonly FadeCurve fade;
    private readonly Piece target;
    private float t => Mathf.Clamp(passedTime / duration, 0, 1);

    private float passedTime;

    public FadeAnimation(FadeCurve fade, Piece target)
    {
        this.fade = fade;
        this.target = target;
        target.Modulate = new Color(1, 1, 1, fade.Value(0));
    }

    public override void _Process(float delta)
    {
        if (!IsInstanceValid(target))
        {
            return;
        }

        passedTime += delta;
        target.Modulate = new Color(1, 1, 1, fade.Value(t));

        if (passedTime >= duration)
        {
            EmitSignal(nameof(Finished));
            QueueFree();
        }
    }
}

public sealed record FadeCurve(float Start, float End, int Steps)
{
    public static FadeCurve FadeIn => new(0, 1, 2);
    public static FadeCurve FadeOut => new(1, 0, 2);

    public float Value(float t)
    {
        var lerp = Start + t * (End - Start);
        return Mathf.Ceil(lerp * Steps) / Steps;
    }
}
