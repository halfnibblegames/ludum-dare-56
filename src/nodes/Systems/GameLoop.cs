using System;
using System.Threading.Tasks;
using Godot;

namespace HalfNibbleGame.Systems;

public sealed class GameLoop : Node2D
{
    private GameLoopState state = GameLoopState.AwaitingInput;

    public InputHandler Input { get; }

    public GameLoop()
    {
        Input = new InputHandler(this);
    }

    public override void _Ready()
    {
        Input.Activate();
    }

    public void SubmitMove(IMove move)
    {
        if (state != GameLoopState.AwaitingInput) throw new InvalidOperationException();

        _ = doPlayerMove(move);
    }

    private async Task doPlayerMove(IMove move)
    {
        state = GameLoopState.PlayerMove;
        Input.Deactivate();
        await move.Execute();

        _ = doEnemyMove();
    }

    private async Task doEnemyMove()
    {
        state = GameLoopState.EnemyMove;
        // TODO: execute enemy moves
        await Task.Delay(100);

        startTurn();
    }

    private void startTurn()
    {
        state = GameLoopState.AwaitingInput;

        // TODO: determine next enemy moves
        Input.Activate();
    }

    private enum GameLoopState
    {
        AwaitingInput,
        PlayerMove,
        EnemyMove,
    }
}
