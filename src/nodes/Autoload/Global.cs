using Godot;
using JetBrains.Annotations;

namespace HalfNibbleGame.Autoload;

// Should always be set to be the first Autoload of a project.
[UsedImplicitly]
public sealed class Global : Node
{
    public static IServiceProvider Services => Instance.services;

    private static Prefabs? prefabs;
    public static Prefabs Prefabs
    {
        get
        {
            prefabs ??= Services.Get<Prefabs>();
            return prefabs;
        }
    }

    // This will be set in _Ready, and since Global is automatically loaded, this will always be true.
    public static Global Instance { get; private set; } = null!;

    private readonly ServiceProvider services = new();

    public override void _Ready()
    {
        Instance = this;
    }

    public void SwitchScene(string path)
    {
        GetTree().ChangeScene(path);
        services.OnSceneChanging();
    }
}
