using Godot;
using JetBrains.Annotations;

namespace HalfNibbleGame.Autoload;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public sealed class Prefabs : Node
{
    [Export] public PackedScene? Tile;

    [Export] public PackedScene? QueenBee;

    public override void _Ready()
    {
        Global.Services.ProvidePersistent(this);
    }
}
