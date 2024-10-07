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
    private readonly Random random;
    private readonly List<Move> enemyMoves = new();
    private Task<MoveContinuation?>? playerMove;
    private readonly List<Task> awaits = new();

    private Levels levels = default!;
    private Board board = default!;
    private readonly List<PackedScene> playerArmy = new();
    private EnemyBrain enemyBrain = default!;
    private GameEnd end = GameEnd.None;
    private GameLoopState state = GameLoopState.Opening;

    private int currentLevel;

    private readonly InputHandler input;

    public GameLoop()
    {
        random = new Random();
        input = new InputHandler(this);
    }

    public override void _Ready()
    {
        levels = new Levels();
        board = GetNode<Board>("Board");
        enemyBrain = new EnemyBrain(board, random);
        resetProgress();
        startGame();
    }

    private void startGame()
    {
        state = GameLoopState.Opening;
        end = GameEnd.None;
        awaits.Add(startGameAsync(levels.All[currentLevel]));
    }

    private async Task startGameAsync(Levels.Level level)
    {
        await board.Reset();
        await deployPieces(level);
    }

    private void endGame()
    {
        state = GameLoopState.Ended;
        input.Deactivate();
        enemyMoves.Clear();

        awaits.Add(board.Disappear());
    }

    private async Task deployPieces(Levels.Level level)
    {
        // Player pieces
        var playerPieces = playerArmy.Select(u => u.Instance<Piece>()).ToList();
        var pieceLocations = layOutPieces(playerPieces, 0, 1);
        await deployPiecesOnBoard(pieceLocations);

        // Enemy pieces
        var enemyPieces = new List<PieceAndLocation>();
        foreach (var u in level.EnemyForce)
        {
            var piece = u.Prefab.Instance<Piece>();
            piece.IsEnemy = true;
            enemyPieces.Add(new PieceAndLocation(piece, u.Location));
        }
        await deployPiecesOnBoard(enemyPieces);
    }

    private static readonly int[] deployOrderInRow = { 3, 4, 2, 5, 1, 6, 0, 7 };

    private IReadOnlyList<PieceAndLocation> layOutPieces(IEnumerable<Piece> pieces, int startRow, int yUp)
    {
        var sortedPieces = pieces.OrderByDescending(p => p.Value).ThenBy(p => p.DisplayName).ToList();

        var row = startRow;
        var i = 0;

        var pieceLocations = new List<PieceAndLocation>();

        foreach (var p in sortedPieces)
        {
            var coord = new TileCoord(deployOrderInRow[i++], row);
            pieceLocations.Add(new PieceAndLocation(p, coord));
            if (i >= deployOrderInRow.Length)
            {
                row += yUp;
                i = 0;
            }
        }

        return pieceLocations.AsReadOnly();
    }

    private async Task deployPiecesOnBoard(IEnumerable<PieceAndLocation> pieces)
    {
        var pieceAnimations = new List<Task>();

        foreach (var p in pieces)
        {
            pieceAnimations.Add(board.AddPiece(p.Piece, p.Tile));
        }

        await Task.WhenAll(pieceAnimations);
    }

    public override void _Process(float delta)
    {
        switch (state)
        {
            case GameLoopState.Opening:
                if (stillWaiting()) break;
                input.Reset();
                startTurn();
                break;
            case GameLoopState.AwaitingInput:
                break;
            case GameLoopState.PlayerMove:
                if (!playerMove!.IsCompleted) break;
                updateGameEnd();
                if (end != GameEnd.None)
                {
                    endGame();
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
                updateGameEnd();
                if (end != GameEnd.None)
                {
                    endGame();
                    break;
                }

                startTurn();
                break;
            case GameLoopState.Ended:
                if (stillWaiting()) break;

                if (end == GameEnd.Win)
                {
                    currentLevel++;

                    if (currentLevel == levels.All.Length)
                    {
                        winGame();
                    }
                    else
                    {
                        showChoices();
                    }

                    break;
                }

                resetProgress();
                startGame();
                break;
            case GameLoopState.AwaitingChoice:
                if (stillWaiting()) break;

                startGame();
                break;
            case GameLoopState.YouAreWinner:
                // TODO: a thing
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
            end = GameEnd.Loss;
            endGame();
        }
        if (@event is InputEventKey { Pressed: true, Scancode: (int) KeyList.F2 })
        {
            end = GameEnd.Win;
            endGame();
        }
    }

    private void updateGameEnd()
    {
        if (end != GameEnd.None) throw new InvalidOperationException();

        var groupedPieces = board.Pieces.ToLookup(p => p.IsEnemy);

        // Check victory
        if (!groupedPieces[true].Any())
        {
            end = GameEnd.Win;
        }

        if (!groupedPieces[false].Any(p => p is QueenBee))
        {
            end = GameEnd.Loss;
        }
    }

    public void SubmitMove(Move move)
    {
        if (state != GameLoopState.AwaitingInput) throw new InvalidOperationException();

        doPlayerMove(move);
    }

    private void doPlayerMove(Move move)
    {
        state = GameLoopState.PlayerMove;
        input.Deactivate();
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
        state = GameLoopState.AwaitingInput;
        determineEnemyMove();
        foreach (var piece in board.Pieces)
        {
            piece.OnTurnStart();
        }
        input.Activate();
    }

    private void continueTurn(MoveContinuation continuation)
    {
        state = GameLoopState.AwaitingInput;
        input.Activate();
        input.SetContinuation(board, continuation);
    }

    private void determineEnemyMove()
    {
        var plannedMoves = enemyBrain.PlanMoves().ToList();
        enemyMoves.AddRange(plannedMoves);
        foreach (var m in plannedMoves)
        {
            m.Piece.IsPrimed = true;
        }
    }

    private void showChoices()
    {
        state = GameLoopState.AwaitingChoice;

        var leftChoice = new Choice(Global.Prefabs.Dragonfly!, Cards.GetRandomCard());
        var rightChoice = new Choice(Global.Prefabs.HornedBeetle!, Cards.GetRandomCard());
        var tcs = new TaskCompletionSource<object?>();
        awaits.Add(tcs.Task);

        Global.Services.Get<ChoicesScreen>().ShowChoices(leftChoice, rightChoice, consumeChoice);

        return;

        void consumeChoice(Choice choice)
        {
            playerArmy.Add(choice.PiecePrefab);
            var cardService = Global.Services.Get<CardService>();
            if (cardService.FirstAvailableSlot() is { } slot)
            {
                cardService.AddCardToSlot(choice.Card, slot);
            }
            tcs.SetResult(null);
        }
    }

    private void resetProgress()
    {
        currentLevel = 0;
        playerArmy.Clear();
        playerArmy.AddRange(levels.InitialArmy);
        Global.Services.Get<CardService>().ResetCards();
        if (Global.Services.TryGet<Chronometer>(out var chronometer))
        {
            chronometer.ResetTime();
        }
    }

    private void winGame()
    {
        state = GameLoopState.YouAreWinner;
        // TODO: fireworks or something?
    }

    private enum GameLoopState
    {
        Opening,
        AwaitingInput,
        PlayerMove,
        EnemyMove,
        Ended,
        AwaitingChoice,
        YouAreWinner
    }

    private enum GameEnd
    {
        None,
        Win,
        Loss
    }

    private sealed record PieceAndLocation(Piece Piece, TileCoord Tile);
}
