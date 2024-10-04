using Godot;
using HalfNibbleGame.Autoload;
using JetBrains.Annotations;

namespace HalfNibbleGame.Controls;

[UsedImplicitly]
public class MasterVolumeSlider : HSlider
{
    public override void _Ready()
    {
        Step = 0.05;
        MinValue = 0.0f;
        MaxValue = 1.0f;
        
        var audioService = Global.Services.Get<AudioService>();
        Value = audioService.MasterVolume;
    }

    [UsedImplicitly]
    public void OnValueChanged(float newValue)
    {
        var audioService = Global.Services.Get<AudioService>();
        audioService.SetVolume(newValue);
    }
}
