using Godot;
using JetBrains.Annotations;

namespace HalfNibbleGame.Autoload;

[UsedImplicitly]
public sealed class Prefabs : Node
{
    public override void _Ready()
    {
        Global.Services.ProvidePersistent(this);
    }
}
