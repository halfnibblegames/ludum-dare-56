using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects.Pieces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalfNibbleGame.Objects.Cards;

public sealed class HornyJail : Card
{
    public HornyJail() : base("Horny Jail") { }
    public override Texture GetTexture() => Global.Prefabs.HornyJail!;

    public override string HelpText => "Stuns all Beetles for 4 turns.\n[b]Bonk, to horny jail.[/b]";

    public override Task<bool> Use(Board board)
    {
        foreach (var beetle in board.Pieces.OfType<HornedBeetle>())
        {
            beetle.Stun(4);
        }

        return Task.FromResult(true);
    }
}
