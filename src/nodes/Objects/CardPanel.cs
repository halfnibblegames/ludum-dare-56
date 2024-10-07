using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects.Cards;
using JetBrains.Annotations;

namespace HalfNibbleGame.Objects;

[UsedImplicitly]
public sealed class CardPanel : Control
{
    private CardSlot slotOne;
    private CardSlot slotTwo;
    private CardSlot slotThree;
    
    public override void _Ready()
    {
        var cardService = Global.Services.Get<CardService>();
        cardService.OnCardListUpdated += OnCardListUpdated;

        slotOne = GetNode<CardSlot>("SlotOne");
        slotOne.Slot = CardService.Slot.One;
        slotTwo = GetNode<CardSlot>("SlotTwo");
        slotTwo.Slot = CardService.Slot.Two;
        slotThree = GetNode<CardSlot>("SlotThree");
        slotThree.Slot = CardService.Slot.Three;
    }

    private void OnCardListUpdated(Dictionary<CardService.Slot, Card> cards)
    {
        slotOne.SetCard(cards.TryGetValue(CardService.Slot.One, out var card) ? card : null);
        slotTwo.SetCard(cards.TryGetValue(CardService.Slot.Two, out var cardTwo) ? cardTwo : null);
        slotThree.SetCard(cards.TryGetValue(CardService.Slot.Three, out var cardThree) ? cardThree : null);
    }
}
