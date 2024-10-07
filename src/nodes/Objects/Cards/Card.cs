using System;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Cards;

public abstract class Card : IHelpable
{
    public string DisplayName { get; }
    public abstract Texture GetTexture();

    protected Card(string name)
    {
        DisplayName = name;
    }

    public abstract Task Use(Board board);

    public abstract string HelpText { get; }
}

public abstract class CardWithTarget : Card
{
    protected CardWithTarget(string name) : base(name) { }

    public Tile? TargetTile { get; set; }

    public override Task Use(Board board)
    {
        if (TargetTile is null)
            throw new InvalidOperationException("Trying to use a card with target without a target.");

        return Use(board, TargetTile);
    }

    protected abstract Task Use(Board board, Tile target);
}

