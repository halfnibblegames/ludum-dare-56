using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects;
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
    private GameLoopState state = GameLoopState.AwaitingInput;

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
        deployPieces();
        startTurn();
    }

    private void deployPieces()
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

        deployPiecesOnBoard(playerPieces, 0, 1);

        // Enemy pieces
        var enemyPieces = new List<Piece>();

        for (var i = 0; i < enemyPieceCount; i++)
        {
            var piece = pieceTypes[random.Next(pieceTypes.Count)].Instance<Piece>();
            piece.IsEnemy = true;
            enemyPieces.Add(piece);
        }

        deployPiecesOnBoard(enemyPieces, 7, -1);
    }

    private static readonly int[] deployOrderInRow = { 3, 4, 2, 5, 1, 6, 0, 7 };

    private void deployPiecesOnBoard(IEnumerable<Piece> pieces, int startRow, int yUp)
    {
        var sortedPieces = pieces.OrderByDescending(p => p.Value).ThenBy(p => p.DisplayName).ToList();

        var row = startRow;
        var i = 0;

        foreach (var p in sortedPieces)
        {
            var coord = new TileCoord(deployOrderInRow[i++], row);
            board.AddPiece(p, coord);
            if (i >= deployOrderInRow.Length)
            {
                row += yUp;
                i = 0;
            }
        }
    }

    public override void _Process(float delta)
    {
        checkGameEnd();
    }

    private void checkGameEnd()
    {
        var groupedPieces = board.Pieces.ToLookup(p => p.IsEnemy);

        // Check victory
        if (!groupedPieces[true].Any())
        {
            // Win!
            Global.Instance.SwitchScene("res://scenes/Game.tscn");
        }

        if (!groupedPieces[false].Any(p => p is QueenBee))
        {
            // Loss!
            Global.Instance.SwitchScene("res://scenes/Game.tscn");
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
        enemyMoves.Clear();
        await Task.WhenAll(moveExecutions);

        startTurn();
    }

    private void startTurn()
    {
        state = GameLoopState.AwaitingInput;
        determineEnemyMove();
        foreach (var piece in GetNode<Board>("Board").Pieces)
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
        var plannedMoves = enemyBrain.PlanMoves().ToList();
        enemyMoves.AddRange(plannedMoves);
        foreach (var m in plannedMoves)
        {
            m.Piece.IsPrimed = true;
        }
    }

    private enum GameLoopState
    {
        AwaitingInput,
        PlayerMove,
        EnemyMove,
    }
}
