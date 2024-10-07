using Godot;
using JetBrains.Annotations;

namespace HalfNibbleGame.Autoload;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public sealed class Prefabs : Node
{
    [Export] public PackedScene? Tile;

    // Units
    [Export] public PackedScene? QueenBee;
    [Export] public PackedScene? Ant;
    [Export] public PackedScene? Dragonfly;
    [Export] public PackedScene? Grasshopper;
    [Export] public PackedScene? HornedBeetle;
    [Export] public PackedScene? PrayingMantis;

    // Card art
    [Export] public Texture? Feature;
    [Export] public Texture? RoyalGuard;
    [Export] public Texture? Sugar;
    [Export] public Texture? MagnifierGlass;
    [Export] public Texture? HornyJail;
    [Export] public Texture? Hivemind;
    [Export] public Texture? RoyalSuccession;
    [Export] public Texture? CitronellaSpiral;

    // Effects
    [Export] public PackedScene? CaptureExplosion;

    public override void _Ready()
    {
        Global.Services.ProvidePersistent(this);
    }
}
