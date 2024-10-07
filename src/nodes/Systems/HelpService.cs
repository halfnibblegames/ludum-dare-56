using Godot;
using HalfNibbleGame.Autoload;
using JetBrains.Annotations;

namespace HalfNibbleGame.Systems;

[UsedImplicitly]
public sealed class HelpService : Node
{
    private Label name = default!;
    private RichTextLabel description = default!;
    
    public override void _Ready()
    {
        Global.Services.ProvideInScene(this);

        name = GetNode<Label>("../HelpedItemName");
        description = GetNode<RichTextLabel>("../HelpedItemDescription");
    }

    public void ShowHelp(IHelpable helpable)
    {
        name.Text = helpable.DisplayName;
        description.BbcodeText = helpable.HelpText;
    }

    public void ClearHelp()
    {
        name.Text = "";
        description.Text = "Hover the mouse over (almost) anything and more information will appear here if available!";
    }
}

public interface IHelpable
{
    public string DisplayName { get; }
    public string HelpText { get; }
}
