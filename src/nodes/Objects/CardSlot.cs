using Godot;
using System;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects.Cards;

public class CardSlot : TextureButton
{
    private bool hasCard;
    public CardService.Slot Slot;

    public override void _Ready()
    {
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left })
            return;

        if (!hasCard)
            return;

        // TODO: Maybe warn the state machine?
        _ = Global.Services.Get<CardService>().UseCardInSlot(Slot);
    }

    public void SetCard(Card? card)
    {
        hasCard = card is not null;
        TextureNormal = card?.GetTexture();
    }
}
