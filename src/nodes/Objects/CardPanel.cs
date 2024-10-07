using System.Linq;
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
    private TextureRect cursorRect = default!;

    private CardSlot? hoveringCard;
    
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

        cursorRect = GetNode<TextureRect>("HoverFrame");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        var cardService = Global.Services.Get<CardService>();
        var cardInUse = cardService.CardInUse;
        var slots = new[] { slotOne, slotTwo, slotThree };
        var slotInUse = cardInUse is null
            ? null
            : slots.FirstOrDefault(s => cardService.GetCardInSlotOrDefault(s.Slot) == cardInUse);
        var isHoveredSlotOccupied = hoveringCard is not null && cardService.GetCardInSlotOrDefault(hoveringCard.Slot) is not null;
        var slotToHighlight = slotInUse ?? (isHoveredSlotOccupied ? hoveringCard : null);

        if (slotToHighlight is null)
        {
            cursorRect.Visible = false;
        }
        else
        {
            cursorRect.RectPosition = slotToHighlight.RectPosition - new Vector2(2, 2);
            cursorRect.Visible = true;
        }
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
        hoveringCard = slotOne;
    }

    [UsedImplicitly]
    public void OnSlotTwoHoverEntered()
    {
        HoverStarted(CardService.Slot.Two);
        hoveringCard = slotTwo;
    }

    [UsedImplicitly]
    public void OnSlotThreeHoverEntered()
    {
        HoverStarted(CardService.Slot.Three);
        hoveringCard = slotThree;
    }

    private void HoverStarted(CardService.Slot slot)
    {
        var cardService = Global.Services.Get<CardService>();
        var helpService = Global.Services.Get<HelpService>();

        var card = cardService.GetCardInSlotOrDefault(slot);
        if (card is null)
        {
            helpService.ClearHelp();
        }
        else
        {
            helpService.ShowHelp(card);
        }
    }

    [UsedImplicitly]
    public void OnSlotHoverExited()
    {
        Global.Services.Get<HelpService>().ClearHelp();
        hoveringCard = null;
    }
}
