using Godot;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame;

public sealed class WorldClickable : Area2D
{
    public override void _InputEvent(Object viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton { Pressed: true })
        {
            Global.Services.Get<ShakeCamera>().Shake(1);
        }
    }
}
