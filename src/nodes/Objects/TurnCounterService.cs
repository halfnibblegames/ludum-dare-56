using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects;

public sealed class TurnCounterService : Node
{
    public int CurrentTurn { get; set; }
    public int CurrentWave { get; set; }

    private Label waveLabel = default!;
    private Label turnLabel = default!;
    
    public override void _Ready()
    {
        base._Ready();
        Global.Services.ProvideInScene(this);

        waveLabel = GetNode<Label>("../WaveLabel");
        turnLabel = GetNode<Label>("../TurnLabel");
    }

    public void OnTurnStart()
    {
        CurrentTurn++;
        turnLabel.Text = $"Turn {CurrentTurn}";
    }

    public void OnBattleStart(Levels.Level level)
    {
        CurrentWave++;
        waveLabel.Text = $"Wave {level.Number}";

        CurrentTurn = 1;
        turnLabel.Text = $"Turn {CurrentTurn}";
    }

    public void Reset()
    {
        CurrentWave = 0;
        CurrentTurn = 1;
    }
}
