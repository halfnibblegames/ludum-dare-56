﻿using Godot;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame.Systems;

public sealed class HelpService : Node
{
    private Label name;
    private Label description;
    
    public override void _Ready()
    {
        Global.Services.ProvideInScene(this);

        name = GetNode<Label>("../HelpedItemName");
        description = GetNode<Label>("../HelpedItemDescription");
    }

    public void ShowHelp(IHelpable helpable)
    {
        name.Text = helpable.Name;
        description.Text = helpable.HelpText;
    }

    public void ClearHelp()
    {
        name.Text = "";
        description.Text = "";
    }
}

public interface IHelpable
{
    public string Name { get; }
    public string HelpText { get; }
}