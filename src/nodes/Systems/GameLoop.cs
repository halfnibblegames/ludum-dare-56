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
    private readonly List<Move> enemyMoves = new();

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

    public void SubmitMove(Move move)
    {
        if (state != GameLoopState.AwaitingInput) throw new InvalidOperationException();

        _ = doPlayerMove(move);
    }

    private async Task doPlayerMove(Move move)
    {
        state = GameLoopState.PlayerMove;
        Input.Deactivate();
        await move.Execute();

        _ = doEnemyMove();
    }

    private async Task doEnemyMove()
    {
        state = GameLoopState.EnemyMove;

        var moveExecutions = enemyMoves.Where(m => m.Validate()).Select(m => m.Execute()).ToList();
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

        const int maxTries = 5;
        for (var i = 0; i < maxTries; i++)
        {
            var target = reachableTiles[random.Next(reachableTiles.Count)];
            var moveCandidate = board.PreviewMove(piece, tile, board[target]);
            if (moveCandidate.Validate())
            {
                enemyMoves.Add(moveCandidate);
                break;
            }
        }
    }

    private enum GameLoopState
    {
        AwaitingInput,
        PlayerMove,
        EnemyMove,
    }
}
