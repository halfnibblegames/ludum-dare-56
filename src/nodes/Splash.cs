using Godot;
using HalfNibbleGame.Autoload;
using JetBrains.Annotations;

[UsedImplicitly]
public class Splash : Control
{
    private const float animationDelay = 0.7f;

    private float elapsedTime;
    private bool animationStarted;

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        if (animationStarted)
            return;

        elapsedTime += delta;
        if (animationDelay > elapsedTime)
            return;

        animationStarted = true;
        GetNode<AudioStreamPlayer>("BiteSound").Play();
        GetNode<AnimatedSprite>("Strawberry").Play("Bite");
    }

    [UsedImplicitly]
    public void OnBiteAnimationFinished()
    {
        var tween = GetNode<Tween>("TextTween");
        const float durationSeconds = 0.8f;
        tween.InterpolateProperty(
            GetNode("Copyright"),
            "percent_visible",
            0,
            1,
            durationSeconds
        );
        tween.Start();
    }

    [UsedImplicitly]
    public void OnTextShown()
    {
        var fadeRect = GetNode<ColorRect>("FadeRect");
        var background = GetNode<ColorRect>("Background");
        var tween = GetNode<Tween>("FadeTween");
        const float durationSeconds = 1.0f;
        tween.InterpolateProperty(
            GetNode("FadeRect"),
            "color",
            fadeRect.Color,
            background.Color,
            durationSeconds
        );
        tween.Start();
    }

    [UsedImplicitly]
    public void OnFadeEnded()
    {
        Global.Instance.SwitchScene("res://scenes/Title.tscn");
    }
}
