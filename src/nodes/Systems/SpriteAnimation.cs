using Godot;

namespace HalfNibbleGame.Systems;

sealed class SpriteAnimation : Node
{
    [Signal] public delegate void Finished();

    private readonly AnimatedSprite sprite;
    private readonly string animation;

    public SpriteAnimation(AnimatedSprite sprite, string animation)
    {
        this.sprite = sprite;
        this.animation = animation;
    }

    public override void _Ready()
    {
        base._Ready();
        sprite.Visible = true;
        sprite.Frame = 0;
        sprite.Play(animation);
        sprite.Connect("animation_finished", this, nameof(onAnimationFinished));
    }

    private void onAnimationFinished()
    {
        sprite.Disconnect("animation_finished", this, nameof(onAnimationFinished));
        EmitSignal(nameof(Finished));
        sprite.Stop();
        sprite.Visible = false;
        QueueFree();
    }
}
