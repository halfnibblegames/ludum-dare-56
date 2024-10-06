using Godot;
using System;
using System.Linq;
using Godot.Collections;
using HalfNibbleGame.Autoload;

public sealed class Boombox : Node
{
    private Dictionary<SoundEffect, AudioStreamPlayer> sfxs = new();

    public override void _Ready()
    {
        Global.Services.ProvideInScene(this);
        foreach (var soundEffect in Enum.GetValues(typeof(SoundEffect)).Cast<SoundEffect>())
        {
            sfxs[soundEffect] = GetNode<AudioStreamPlayer>(soundEffect.ToString());
        }
    }

    public void Play(SoundEffect soundEffect)
    {
        var sfxToPlay = sfxs[soundEffect];
        if (!sfxToPlay.Playing)
        {
            sfxToPlay.Play();
        }
    }

    public enum SoundEffect
    {
        Select,
        Capture,
        Swipe,
        Walk,
    }
}
