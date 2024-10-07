using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

sealed class ChoicePanel : Control
{
    private TextureButton pieceSlot = default!;
    private CardSlot cardSlot = default!;

    private Piece? spawnedPiece;
    private Choice? choice;

    public Choice? Choice
    {
        get => choice;
        set
        {
            if (choice == value) return;
            choice = value;
            cardSlot.SetCard(choice?.Card);
            spawnedPiece?.QueueFree();
            spawnedPiece = null;
            if (choice == null) return;
            spawnedPiece = choice.PiecePrefab.Instance<Piece>();
            pieceSlot.AddChild(spawnedPiece);
            spawnedPiece.Position = new Vector2(9, 10);
        }
    }

    public override void _Ready()
    {
        pieceSlot = GetNode<TextureButton>("PieceSlot");
        cardSlot = GetNode<CardSlot>("CardSlot");
        GetNode<Control>("Background/Hover").Visible = false;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left }
            or InputEventScreenTouch { Pressed: true })
        {
            Global.Services.Get<ChoicesScreen>().UseChoice(Choice);
            Global.Services.Get<HelpService>().ClearHelp();
        }
    }

    public void OnMouseEntered()
    {
        GetNode<Control>("Background/Hover").Visible = true;
    }

    public void OnMouseExited()
    {
        GetNode<Control>("Background/Hover").Visible = false;
    }

    public void OnPieceSlotMouseEntered()
    {
        if (spawnedPiece is null) return;
        Global.Services.Get<HelpService>().ShowHelp(spawnedPiece);
    }

    public void OnCardSlotMouseEntered()
    {
        if (choice is null) return;
        Global.Services.Get<HelpService>().ShowHelp(choice.Card);
    }

    public void OnSlotMouseExited()
    {
        Global.Services.Get<HelpService>().ClearHelp();
    }
}
