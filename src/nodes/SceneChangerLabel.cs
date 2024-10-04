using Godot;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame;

public sealed class SceneChangerLabel : Label
{
    [Export]
    public string? TargetScenePath;

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);

        if (TargetScenePath is null) return;

        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left })
        {
            return;
        }

        GetTree().SetInputAsHandled();
        Global.Instance.SwitchScene(TargetScenePath);
    }
}
