using Godot;
using JetBrains.Annotations;
using System;

namespace HalfNibbleGame.Autoload;

[UsedImplicitly]
public sealed class AudioService : Node
{
    // Minimum amount of negative decibels accepted by the master bus.
    private const float silence = -80.0f;
    private const string masterBusName = "Master";
    private const string volumeConfigFileLocation = "user://volume.cfg";
    private const string volumeConfigurationSection = "volume";
    private const string masterVolumeBeforeMutingKey = "volume_before_muting";
    private const string masterVolumeConfigurationKey = "master_volume";

    [ValueRange(0, 1)] private float masterVolumeBeforeMuting = 1;

    [ValueRange(0, 1)]
    public float MasterVolume { get; private set; }

    public event Action<float> OnVolumeChanged = delegate { };

    public override void _Ready()
    {
        Global.Services.ProvidePersistent(this);

        var configFile = new ConfigFile();
        var error = configFile.Load(volumeConfigFileLocation);
        if (error is not Error.Ok)
            return;

        MasterVolume = (float) configFile.GetValue(
            section: volumeConfigurationSection,
            key: masterVolumeConfigurationKey,
            @default: 1.0f
        );

        masterVolumeBeforeMuting = (float) configFile.GetValue(
            section: volumeConfigurationSection,
            key: masterVolumeBeforeMutingKey,
            @default: 1.0f
        );
    }

    public void Mute()
    {
        masterVolumeBeforeMuting = MasterVolume;
        SetVolume(0);
    }

    public void Unmute()
    {
        var unmuteVolume = Math.Max(masterVolumeBeforeMuting, 0.1f);
        SetVolume(unmuteVolume);
    }

    public void SetVolume([ValueRange(0, 1)] float volume)
    {
        var masterBusIndex = AudioServer.GetBusIndex(masterBusName);
        var newVolumeInDb = (Math.Abs(silence) * volume) + silence;
        AudioServer.SetBusVolumeDb(masterBusIndex, newVolumeInDb);
        MasterVolume = volume;

        OnVolumeChanged(volume);

        var configFile = new ConfigFile();
        configFile.SetValue(volumeConfigurationSection, masterVolumeConfigurationKey, MasterVolume);
        configFile.SetValue(volumeConfigurationSection, masterVolumeBeforeMutingKey, masterVolumeBeforeMuting);
        configFile.Save(volumeConfigFileLocation);
    }
}
