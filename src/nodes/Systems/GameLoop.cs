using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class GameLoop : Node2D
{
    private readonly Random random;
    private GameLoopState state = GameLoopState.AwaitingInput;
    private readonly List<IMove> enemyMoves = new();

    public InputHandler Input { get; }

    public GameLoop()
    {
        random = new Random();
        Input = new InputHandler(this);
    }

    public override void _Ready()
    {
        startTurn();
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

        var moveExecutions = enemyMoves.Select(m => m.Execute()).ToList();
        enemyMoves.Clear();
        await Task.WhenAll(moveExecutions);

        startTurn();
    }

    private void startTurn()
    {
        state = GameLoopState.AwaitingInput;
        determineEnemyMove();
        Input.Activate();
    }

    private void determineEnemyMove()
    {
        var board = GetNode<Board>("Board");

        var enemyPieces = board.Tiles.Where(t => t.Piece is {IsEnemy: true}).Select(t => (t, t.Piece!)).ToList();
        if (enemyPieces.Count == 0) return;
        var (tile, piece) = enemyPieces[random.Next(enemyPieces.Count)];
        var reachableTiles = piece.ReachableTiles(tile.Coord, board).ToList();
        if (reachableTiles.Count == 0) return;
        var target = reachableTiles[random.Next(reachableTiles.Count)];

        enemyMoves.Add(Moves.MovePiece(piece, tile, board[target]));
    }

    private enum GameLoopState
    {
        AwaitingInput,
        PlayerMove,
        EnemyMove,
    }
}
