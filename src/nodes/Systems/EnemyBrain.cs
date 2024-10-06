using System;
using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

sealed class EnemyBrain
{
    private readonly Board board;
    private readonly Random random;

    public EnemyBrain(Board board, Random random)
    {
        this.board = board;
        this.random = random;
    }

    public IEnumerable<Move> PlanMoves()
    {
        var enemyPieces = board.Tiles
            .Where(t => t.Piece is {IsEnemy: true, IsStunned: false})
            .Select(t => (t, t.Piece!))
            .ToList();
        if (enemyPieces.Count == 0) yield break;

        // 25% chance of moving two pieces.
        var numberOfPieces = random.NextDouble() < 0.25 ? 2 : 1;
        var chosenPieces = enemyPieces
            .Select(tuple => (tuple.Item1, tuple.Item2, tuple.Item2.ReachableTiles(tuple.Item1.Coord, board).ToList()))
            .Where(tuple => tuple.Item3.Count > 0)
            .OrderBy(_ => random.Next())
            .Take(numberOfPieces).ToList();

        var occupiedTiles = new List<TileCoord>();

        foreach (var (tile, piece, reachableTiles) in chosenPieces)
        {
            reachableTiles.RemoveAll(occupiedTiles.Contains);
            if (reachableTiles.Count == 0) continue;

            const int maxTries = 5;
            for (var i = 0; i < maxTries; i++)
            {
                var target = reachableTiles[random.Next(reachableTiles.Count)];
                var moveCandidate = board.PreviewMove(piece, tile, board[target], 0);
                if (moveCandidate.Validate())
                {
                    yield return moveCandidate;
                    occupiedTiles.Add(moveCandidate.To.Coord);
                    moveCandidate.Piece.NextMove = moveCandidate;
                    break;
                }
            }
        }
    }
}
