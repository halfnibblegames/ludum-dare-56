using System;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

sealed class ChoicesScreen : Control
{
    private ChoicePanel leftPanel = default!;
    private ChoicePanel rightPanel = default!;
    private Action<Choice>? consumeChoice;

    public override void _Ready()
    {
        Visible = false;
        Global.Services.ProvideInScene(this);
        leftPanel = GetNode<ChoicePanel>("LeftChoice");
        rightPanel = GetNode<ChoicePanel>("RightChoice");
    }

    public void ShowChoices(Choice left, Choice right, Action<Choice> consume)
    {
        leftPanel.Choice = left;
        rightPanel.Choice = right;
        consumeChoice = consume;
        Visible = true;
    }

    public void UseChoice(Choice? choice)
    {
        if (consumeChoice is null || choice is null) return;

        consumeChoice(choice);
        consumeChoice = null;
        leftPanel.Choice = null;
        rightPanel.Choice = null;
        Visible = false;
    }
}
