using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects.Cards;

namespace HalfNibbleGame.Objects;

public sealed class CardSlot : TextureButton
{
    private bool hasCard;
    public CardService.Slot Slot;

    public override void _Ready()
    {
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);

        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left }
            or InputEventScreenTouch { Pressed: true })
        {
            if (!hasCard)
                return;

            // TODO: Maybe warn the state machine?
            _ = Global.Services.Get<CardService>().UseCardInSlot(Slot);
        }
    }

    public void SetCard(Card? card)
    {
        hasCard = card is not null;
        TextureNormal = card?.GetTexture();
    }
}
