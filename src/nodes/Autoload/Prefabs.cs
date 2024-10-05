using Godot;
using JetBrains.Annotations;

namespace HalfNibbleGame.Autoload;

[UsedImplicitly]
public sealed class Prefabs : Node
{
    [Export] public PackedScene? Tile;

    public override void _Ready()
    {
        Global.Services.ProvidePersistent(this);
    }
}
