using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame.Objects.Cards;

public sealed class CardService : Node
{
    private readonly Dictionary<Slot, Card> slots = new();
    private Board board = default!;
    
    public List<Slot> SlotsInUse => slots.Keys.ToList();
    public Card? GetCardInSlotOrDefault(Slot slot) => slots.TryGetValue(slot, out var card) ? card : null;

    public Card? CardInUse { get; private set; }

    public event Action OnCardListUpdated = () => { };

    public override void _Ready()
    {
        Global.Services.ProvideInScene(this);
        base._Ready();

        board = GetNode<Board>("../Board");
    }

    public async Task UseCardInSlot(Slot slot)
    {
        var possibleCard = slots[slot];
        if (possibleCard is null)
            throw new InvalidOperationException("Trying to use empty slot");

        try
        {
            CardInUse = possibleCard;

            if (CardInUse is CardWithTarget cardWithTarget)
            {
                // TODO: Pick a target properly
                cardWithTarget.TargetTile = board.Tiles.First(x => x.Piece is not null);
            }

            await possibleCard.Use(board);
        }
        finally
        {
            CardInUse = null;
            slots.Remove(slot);
            OnCardListUpdated();
        }
    }

    public void AddCardToSlot(Card card, Slot slot)
    {
        slots[slot] = card;
        OnCardListUpdated();
    }

    public void ResetCards()
    {
        slots.Clear();
        OnCardListUpdated();
    }

    public enum Slot
    {
        One,
        Two,
        Three
    }
}
