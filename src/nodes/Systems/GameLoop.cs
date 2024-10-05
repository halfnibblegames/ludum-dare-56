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
        setupGame();
        startTurn();
    }

    private void setupGame()
    {
        var queen = Global.Prefabs.QueenBee!.Instance<Piece>();
        board.AddPiece(queen, randomPlayerTile());

        var pieceTypes = pieceTypesFactory();

        for (var i = 0; i < 4; i++)
        {
            var piece = pieceTypes[random.Next(pieceTypes.Count)].Instance<Piece>();
            var tile = randomPlayerTile();
            while (board[tile].Piece != null) tile = randomPlayerTile();
            board.AddPiece(piece, tile);
        }

        for (var i = 0; i < 5; i++)
        {
            var piece = pieceTypes[random.Next(pieceTypes.Count)].Instance<Piece>();
            piece.IsEnemy = true;
            var tile = randomEnemyTile();
            while (board[tile].Piece != null) tile = randomEnemyTile();
            board.AddPiece(piece, tile);
        }
    }

    private TileCoord randomPlayerTile() => randomTile(0, 3);
    private TileCoord randomEnemyTile() => randomTile(5, 8);

    private TileCoord randomTile(int minRowInclusive, int maxRowExclusive)
    {
        return new TileCoord(random.Next(Board.Width), random.Next(minRowInclusive, maxRowExclusive));
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
        foreach (var piece in GetNode<Board>("Board").Pieces)
        {
            piece.OnTurnStart();
        }
        Input.Activate();
    }

    private void determineEnemyMove()
    {
        var enemyPieces = board.Tiles
            .Where(t => t.Piece is {IsEnemy: true, IsStunned: false})
            .Select(t => (t, t.Piece!))
            .ToList();
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
