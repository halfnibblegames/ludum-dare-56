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
        _ = startGame();
    }

    private async Task restartGame()
    {
        endGame();
        await startGame();
    }

    private async Task startGame()
    {
        state = GameLoopState.Opening;

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

        startTurn();
    }

    private void endGame()
    {
        state = GameLoopState.Ended;
        Input.Deactivate();
        Input.Reset();
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
        checkGameEnd();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey { Pressed: true, Scancode: (int) KeyList.F1 })
        {
            _ = restartGame();
        }
    }

    private void checkGameEnd()
    {
        if (state != GameLoopState.AwaitingInput) return;

        var groupedPieces = board.Pieces.ToLookup(p => p.IsEnemy);

        // Check victory
        if (!groupedPieces[true].Any())
        {
            // Win!
            _ = restartGame();
        }

        if (!groupedPieces[false].Any(p => p is QueenBee))
        {
            // Loss!
            _ = restartGame();
        }
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
        var continuation = await move.Execute();

        if (continuation is not null)
        {
            continueTurn(continuation);
            return;
        }

        _ = doEnemyMove();
    }

    private async Task doEnemyMove()
    {
        state = GameLoopState.EnemyMove;

        var moveExecutions = new List<Task>();
        foreach (var move in enemyBrain.ImproveMoves(enemyMoves))
        {
            move.Piece.IsPrimed = false;
            if (move.Validate())
            {
                moveExecutions.Add(move.Execute());
            }
        }

        var enemyAwait = Task.WhenAll(moveExecutions);
        var timeout = Task.Delay(5000);

        var completedTask = await Task.WhenAny(enemyAwait, timeout);
        if (completedTask == timeout)
        {
            throw new Exception("Enemy move did not complete within 5 seconds");
        }

        enemyMoves.Clear();
        startTurn();
    }

    private void startTurn()
    {
        GD.Print("Starting new turn");
        state = GameLoopState.AwaitingInput;
        determineEnemyMove();
        GD.Print("Applying turn start to pieces");
        foreach (var piece in board.Pieces)
        {
            piece.OnTurnStart();
        }
        GD.Print("Turn started, activating input");
        Input.Activate();
    }

    private void continueTurn(MoveContinuation continuation)
    {
        GD.Print("Continuing turn");
        state = GameLoopState.AwaitingInput;
        GD.Print("Activating input");
        Input.Activate();
        GD.Print("Forcing input to use continuation");
        Input.SetContinuation(board, continuation);
    }

    private void determineEnemyMove()
    {
        GD.Print("Determining enemy turn");
        var plannedMoves = enemyBrain.PlanMoves().ToList();
        enemyMoves.AddRange(plannedMoves);
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
}
