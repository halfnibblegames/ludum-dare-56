using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects;
using HalfNibbleGame.Objects.Cards;
using HalfNibbleGame.Objects.Pieces;

namespace HalfNibbleGame.Systems;

public sealed class GameLoop : Node2D
{
    // Needs to be lazily initiated
    private static readonly Func<List<PackedScene>> pieceTypesFactory = () =>
        new List<PackedScene>
        {
            Global.Prefabs.Ant!,
            Global.Prefabs.Dragonfly!,
            Global.Prefabs.Grasshopper!,
            Global.Prefabs.HornedBeetle!,
            Global.Prefabs.PrayingMantis!
        };

    private readonly Random random;
    private readonly List<Move> enemyMoves = new();
    private Task<MoveContinuation?>? playerMove;
    private readonly List<Task> awaits = new();

    private Board board = default!;
    private EnemyBrain enemyBrain = default!;
    private GameLoopState state = GameLoopState.Opening;

    public InputHandler Input { get; }

    public GameLoop()
    {
        random = new Random();
        Input = new InputHandler(this);
    }

    public override void _Ready()
    {
        board = GetNode<Board>("Board");
        enemyBrain = new EnemyBrain(board, random);
        startGame();
    }

    private void restartGame()
    {
        endGame();
        startGame();
    }

    private void startGame()
    {
        state = GameLoopState.Opening;
        awaits.Add(startGameAsync());
    }

    private async Task startGameAsync()
    {
        await board.Reset();
        await deployPieces();

        // TODO: Use proper card giving mechanisms, at some point.
        Task.Run(async () =>
        {
            await Task.Delay(1000);
            Global.Services.Get<CardService>().AddCardToSlot(Cards.GetRandomCard(), CardService.Slot.One);
            Global.Services.Get<CardService>().AddCardToSlot(Cards.GetRandomCard(), CardService.Slot.Two);
            Global.Services.Get<CardService>().AddCardToSlot(Cards.GetRandomCard(), CardService.Slot.Three);
        });
    }

    private void endGame()
    {
        state = GameLoopState.Ended;
        Input.Deactivate();
        Input.Reset();
        enemyMoves.Clear();
    }

    private async Task deployPieces()
    {
        const int playerPieceCount = 3;
        const int enemyPieceCount = 6;

        var pieceTypes = pieceTypesFactory();

        // Player pieces
        var playerPieces = new List<Piece>();

        var queen = Global.Prefabs.QueenBee!.Instance<Piece>();
        playerPieces.Add(queen);

        for (var i = 0; i < playerPieceCount; i++)
        {
            playerPieces.Add(pieceTypes[random.Next(pieceTypes.Count)].Instance<Piece>());
        }

        await deployPiecesOnBoard(playerPieces, 0, 1);

        // Enemy pieces
        var enemyPieces = new List<Piece>();

        for (var i = 0; i < enemyPieceCount; i++)
        {
            var piece = pieceTypes[random.Next(pieceTypes.Count)].Instance<Piece>();
            piece.IsEnemy = true;
            enemyPieces.Add(piece);
        }

        await deployPiecesOnBoard(enemyPieces, 7, -1);
    }

    private static readonly int[] deployOrderInRow = { 3, 4, 2, 5, 1, 6, 0, 7 };

    private async Task deployPiecesOnBoard(IEnumerable<Piece> pieces, int startRow, int yUp)
    {
        var sortedPieces = pieces.OrderByDescending(p => p.Value).ThenBy(p => p.DisplayName).ToList();

        var row = startRow;
        var i = 0;

        var pieceAnimations = new List<Task>();

        foreach (var p in sortedPieces)
        {
            var coord = new TileCoord(deployOrderInRow[i++], row);
            pieceAnimations.Add(board.AddPiece(p, coord));
            if (i >= deployOrderInRow.Length)
            {
                row += yUp;
                i = 0;
            }
        }

        await Task.WhenAll(pieceAnimations);
    }

    public override void _Process(float delta)
    {
        switch (state)
        {
            case GameLoopState.Opening:
                if (stillWaiting()) break;
                startTurn();
                break;
            case GameLoopState.AwaitingInput:
                break;
            case GameLoopState.PlayerMove:
                if (!playerMove!.IsCompleted) break;
                if (checkGameEnd() != GameEnd.None)
                {
                    restartGame();
                    break;
                }

                var continuation = playerMove.Result;
                if (continuation is not null)
                {
                    continueTurn(continuation);
                    break;
                }

                doEnemyMove();
                break;
            case GameLoopState.EnemyMove:
                if (stillWaiting()) break;
                if (checkGameEnd() != GameEnd.None)
                {
                    restartGame();
                    break;
                }

                startTurn();
                break;
            case GameLoopState.Ended:
                if (stillWaiting()) break;

                startGame();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool stillWaiting()
    {
        awaits.RemoveAll(t => t.IsCompleted);
        return awaits.Count > 0;
    }

    public override void _Input(InputEvent @event)
    {
        if (state != GameLoopState.AwaitingInput) return;

        if (@event is InputEventKey { Pressed: true, Scancode: (int) KeyList.F1 })
        {
            restartGame();
        }
    }

    private GameEnd checkGameEnd()
    {
        var groupedPieces = board.Pieces.ToLookup(p => p.IsEnemy);

        // Check victory
        if (!groupedPieces[true].Any())
        {
            return GameEnd.Win;
        }

        if (!groupedPieces[false].Any(p => p is QueenBee))
        {
            return GameEnd.Loss;
        }

        return GameEnd.None;
    }

    public void SubmitMove(Move move)
    {
        if (state != GameLoopState.AwaitingInput) throw new InvalidOperationException();

        doPlayerMove(move);
    }

    private void doPlayerMove(Move move)
    {
        state = GameLoopState.PlayerMove;
        Input.Deactivate();
        playerMove = move.Execute();
    }

    private void doEnemyMove()
    {
        state = GameLoopState.EnemyMove;

        foreach (var move in enemyBrain.ImproveMoves(enemyMoves))
        {
            move.Piece.IsPrimed = false;
            if (move.Validate())
            {
                awaits.Add(move.Execute());
            }
        }

        enemyMoves.Clear();
    }

    private void startTurn()
    {
        GD.Print("Starting new turn");
        state = GameLoopState.AwaitingInput;
        determineEnemyMove();
        foreach (var piece in board.Pieces)
        {
            piece.OnTurnStart();
        }
        Input.Activate();
    }

    private void continueTurn(MoveContinuation continuation)
    {
        state = GameLoopState.AwaitingInput;
        Input.Activate();
        Input.SetContinuation(board, continuation);
    }

    private void determineEnemyMove()
    {
        GD.Print($"Determining enemy moves. Starting with {enemyMoves.Count} moves (should be 0)");
        var plannedMoves = enemyBrain.PlanMoves().ToList();
        enemyMoves.AddRange(plannedMoves);
        GD.Print($"Planned {enemyMoves.Count} moves");
        foreach (var m in plannedMoves)
        {
            m.Piece.IsPrimed = true;
        }
    }

    private enum GameLoopState
    {
        Opening,
        AwaitingInput,
        PlayerMove,
        EnemyMove,
        Ended
    }

    private enum GameEnd
    {
        None,
        Win,
        Loss
    }
}
