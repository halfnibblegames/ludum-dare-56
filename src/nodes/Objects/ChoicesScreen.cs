using System;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

sealed class ChoicesScreen : Control
{
    private const float animationDuration = 0.4f;
    private const float animationOffset = 0.15f;
    private const float heightDisplacement = 8;

    private ChoicePanel leftPanel = default!;
    private ChoicePanel rightPanel = default!;
    private Action<Choice>? consumeChoice;

    private Vector2 leftRectStart;
    private Vector2 rightRectStart;
    private Animation activeAnimation;
    private float timeSinceAnimationStart;
    private Choice? currentChoice;

    public override void _Ready()
    {
        Visible = false;
        Global.Services.ProvideInScene(this);
        leftPanel = GetNode<ChoicePanel>("LeftChoice");
        rightPanel = GetNode<ChoicePanel>("RightChoice");
    }

    public override void _Process(float delta)
    {
        if (activeAnimation == Animation.None) return;
        timeSinceAnimationStart += delta;

        if (timeSinceAnimationStart >= animationDuration + animationOffset)
        {
            finishAnimation();
        }

        var tLeft = Mathf.Clamp(timeSinceAnimationStart / animationDuration, 0f, 1f);
        var tRight = Mathf.Clamp((timeSinceAnimationStart - animationOffset) / animationDuration, 0f, 1f);

        if (activeAnimation == Animation.Exit)
        {
            tLeft = 1 - tLeft;
            tRight = 1 - tRight;
        }

        var steppedTLeft = FadeCurve.FadeIn.Value(tLeft);
        var steppedTRight = FadeCurve.FadeIn.Value(tRight);

        leftPanel.Modulate = new Color(1, 1, 1, steppedTLeft);
        rightPanel.Modulate = new Color(1, 1, 1, steppedTRight);
        leftPanel.RectPosition = leftRectStart + (1 - tLeft) * heightDisplacement * Vector2.Down;
        rightPanel.RectPosition = rightRectStart + (1 - tRight) * heightDisplacement * Vector2.Down;
    }

    public void ShowChoices(Choice left, Choice right, Action<Choice> consume)
    {
        leftPanel.Choice = left;
        rightPanel.Choice = right;
        consumeChoice = consume;
        Visible = true;
        MouseFilter = MouseFilterEnum.Ignore;
        startAnimation(Animation.Enter);
    }

    public void UseChoice(Choice? choice)
    {
        if (consumeChoice is null || choice is null || currentChoice is not null) return;

        currentChoice = choice;
        startAnimation(Animation.Exit);
    }

    private void startAnimation(Animation animation)
    {
        if (activeAnimation != Animation.None) throw new InvalidOperationException();

        leftRectStart = leftPanel.RectPosition;
        rightRectStart = rightPanel.RectPosition;
        activeAnimation = animation;
        timeSinceAnimationStart = 0;
    }

    private void finishAnimation()
    {
        var finishedAnimation = activeAnimation;
        activeAnimation = Animation.None;

        switch (finishedAnimation)
        {
            case Animation.Enter:
                MouseFilter = MouseFilterEnum.Stop;
                break;
            case Animation.Exit:
                finalizeChoice();
                break;
            case Animation.None:
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    private void finalizeChoice()
    {
        consumeChoice!(currentChoice!);
        consumeChoice = null;
        currentChoice = null;
        leftPanel.Choice = null;
        rightPanel.Choice = null;
        Visible = false;
        activeAnimation = Animation.None;
    }

    private enum Animation
    {
        None,
        Enter,
        Exit
    }
}
