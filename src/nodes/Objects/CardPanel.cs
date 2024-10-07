using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects.Cards;
using HalfNibbleGame.Systems;
using JetBrains.Annotations;

namespace HalfNibbleGame.Objects;

[UsedImplicitly]
public sealed class CardPanel : Control
{
    private CardSlot slotOne = default!;
    private CardSlot slotTwo = default!;
    private CardSlot slotThree = default!;
    
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

    private void OnCardListUpdated()
    {
        var cardService = Global.Services.Get<CardService>();
        slotOne.SetCard(cardService.GetCardInSlotOrDefault(CardService.Slot.One));
        slotTwo.SetCard(cardService.GetCardInSlotOrDefault(CardService.Slot.Two));
        slotThree.SetCard(cardService.GetCardInSlotOrDefault(CardService.Slot.Three));
    }
    
    [UsedImplicitly]
    public void OnSlotOneHoverEntered()
    {
        HoverStarted(CardService.Slot.One);
    }

    [UsedImplicitly]
    public void OnSlotTwoHoverEntered()
    {
        HoverStarted(CardService.Slot.Two);
    }

    [UsedImplicitly]
    public void OnSlotThreeHoverEntered()
    {
        HoverStarted(CardService.Slot.Three);
    }

    private void HoverStarted(CardService.Slot slot)
    {
        var card = Global.Services.Get<CardService>().GetCardInSlotOrDefault(slot);
        if (card is null)
        {
            Global.Services.Get<HelpService>().ClearHelp();
        }
        else
        {
            Global.Services.Get<HelpService>().ShowHelp(card);
        }
    }

    [UsedImplicitly]
    public void OnSlotHoverExited()
    {
        Global.Services.Get<HelpService>().ClearHelp();
    }
}
