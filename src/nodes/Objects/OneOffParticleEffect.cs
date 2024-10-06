using System.Linq;
using Godot;

namespace HalfNibbleGame.Objects;

sealed class OneOffParticleEffect : Node2D
{
    [Export] public float Duration { get; set; } = 1;
    private float timePassed;

    public override void _Ready()
    {
        foreach (var c in GetChildren().Cast<Node>().Where(n => n is CPUParticles2D))
        {
            var particleSystem = (CPUParticles2D) c;
            particleSystem.Emitting = true;
        }
    }

    public override void _Process(float delta)
    {
        timePassed += delta;
        if (timePassed >= Duration)
        {
            QueueFree();
        }
    }
}
