using Godot;
using HalfNibbleGame.Autoload;
using System.Threading.Tasks;

namespace HalfNibbleGame.Objects.Cards;

public sealed class CitronellaSpiral : CardWithTarget
{
    public CitronellaSpiral() : base("Citronella Spiral") { }
    public override Texture GetTexture() => Global.Prefabs.CitronellaSpiral!;

    public override string HelpText => "Target can't be captured until the end of the your next turn.\n[b]Do these things even work?[/b]";

    protected override Task Use(Board board, Tile target)
    {
        //TODO: Implement
        return Task.CompletedTask;
    }
}
