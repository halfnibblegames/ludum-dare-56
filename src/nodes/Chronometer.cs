using System;
using Godot;
using HalfNibbleGame.Autoload;
using JetBrains.Annotations;

namespace HalfNibbleGame;

[UsedImplicitly]
public sealed class Chronometer : Label
{
    private TimeSpan elapsedTime = TimeSpan.Zero;
    private bool isStopped;

    public override void _Ready()
    {
        Global.Services.ProvideInScene(this);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!isStopped)
        {
            elapsedTime += TimeSpan.FromSeconds(delta);
        }

        Text = $"{(elapsedTime.Hours > 0 ? $"{elapsedTime.Hours}:" : "")}{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }

    public void Stop()
    {
        isStopped = true;
    }

    public void ResetTime()
    {
        elapsedTime = TimeSpan.Zero;
        isStopped = false;
    }
}
