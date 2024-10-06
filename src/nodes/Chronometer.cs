using System;
using Godot;
using JetBrains.Annotations;

namespace HalfNibbleGame;

[UsedImplicitly]
public sealed class Chronometer : Label
{
    private TimeSpan elapsedTime = TimeSpan.Zero;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        elapsedTime += TimeSpan.FromSeconds(delta);
        
        Text = $"{(elapsedTime.Hours > 0 ? $"{elapsedTime.Hours}:" : "")}{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }
}
