using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects.Pieces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalfNibbleGame.Objects.Cards;

public sealed class RoyalSuccession : Card
{
    public RoyalSuccession() : base("Royal Succession") { }
    public override Texture GetTexture() => Global.Prefabs.RoyalSuccession!;

    public override string HelpText => "The queen re-spawns once on a random tile if it’s killed during this wave.\n[b]The Queen is Dead. Long live the Queen.[/b]";

    public override Task Use(Board board)
    {
        var queenBee = board.Pieces.Single(x => x is QueenBee);
        queenBee.RevivesOnDeath = true;

        return Task.CompletedTask;
    }
}
