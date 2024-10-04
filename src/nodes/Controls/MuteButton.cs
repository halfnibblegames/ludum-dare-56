using Godot;
using HalfNibbleGame.Autoload;

public class MuteButton : TextureButton
{
    public bool Muted { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        
        
        var service = Global.Services.Get<AudioService>();
        service.OnVolumeChanged += OnVolumeChanged;
    }

    protected override void Dispose(bool disposing)
    {
        var service = Global.Services.Get<AudioService>();
        service.OnVolumeChanged -= OnVolumeChanged;

        base.Dispose(disposing);
    }

    private void OnVolumeChanged(float newVolume)
    {
        Muted = newVolume == 0.0f;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left })
            return;

        var service = Global.Services.Get<AudioService>();
        if (Muted)
        {
            Muted = false;
            service.Unmute();
        }
        else
        {
            service.Mute();
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
