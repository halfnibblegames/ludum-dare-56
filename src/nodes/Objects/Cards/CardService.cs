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

    public Card? GetCardInSlotOrDefault(Slot slot) => slots.TryGetValue(slot, out var card) ? card : null;

    public Card? CardInUse { get; private set; }

    public event Action OnCardListUpdated = () => { };

    public override void _Ready()
    {
        Global.Services.ProvideInScene(this);
        base._Ready();

        board = GetNode<Board>("../Board");
    }

    public Slot? FirstAvailableSlot()
    {
        return Enum.GetValues(typeof(Slot)).Cast<Slot?>().FirstOrDefault(s => !slots.Keys.Contains(s!.Value));
    }

    public async Task UseCardInSlot(Slot slot)
    {
        var possibleCard = slots[slot];
        if (possibleCard is null)
            throw new InvalidOperationException("Trying to use empty slot");

        var result = false;
        try
        {
            CardInUse = possibleCard;
            result = await possibleCard.Use(board);
        }
        finally
        {
            CardInUse = null;
            if (result)
            {
                slots.Remove(slot);
                OnCardListUpdated();
            }
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
