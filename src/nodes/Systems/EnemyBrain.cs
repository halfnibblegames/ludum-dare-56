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
        var pieces = enumeratePieces().ToList();
        if (pieces.Count == 0) yield break;

        var bestMovePerPiece = pieces.ToDictionary(
            piece => piece,
            piece =>
            {
                var moves = enumerateMoves(piece);
                var bestMove = moves.OrderByDescending(m => m.HeuristicScore).FirstOrDefault();
                return bestMove;
            });

        var orderedPieces = pieces
            .Where(p => bestMovePerPiece[p] is not null)
            .OrderByDescending(p => bestMovePerPiece[p]!.HeuristicScore)
            .ToList();

        // 25% chance of moving two pieces.
        var numberOfPieces = random.NextDouble() < 0.25 ? 2 : 1;

        var occupiedTiles = new List<TileCoord>();
        foreach (var piece in orderedPieces)
        {
            var candidateMove = bestMovePerPiece[piece]!;

            // Avoid multiple pieces trying to move to the same other tile
            if (occupiedTiles.Contains(candidateMove.Move.To.Coord)) continue;

            yield return candidateMove.Move;
            occupiedTiles.Add(candidateMove.Move.To.Coord);
            piece.Piece.NextMove = candidateMove.Move;

            numberOfPieces--;
            if (numberOfPieces == 0) break;
        }
    }

    private IEnumerable<PlacedPiece> enumeratePieces()
    {
        return board.Tiles
            .Where(t => t.Piece is { IsEnemy: true, IsStunned: false })
            .Select(t => PlacedPiece.FromTile(board, t))
            .Where(p => p.ReachableTiles.Count > 0);
    }

    private IEnumerable<MoveCandidate> enumerateMoves(PlacedPiece piece)
    {
        foreach (var t in piece.ReachableTiles)
        {
            var move = board.PreviewMove(piece.Piece, piece.Tile, board[t], 0);
            var result = move.Preview();
            yield return new MoveCandidate(move, result);
        }
    }

    private sealed record PlacedPiece(Piece Piece, Tile Tile, IReadOnlyList<TileCoord> ReachableTiles)
    {
        public static PlacedPiece FromTile(Board board, Tile tile) =>
            new(tile.Piece!, tile, tile.Piece!.ReachableTiles(tile.Coord, board).ToList().AsReadOnly());
    }

    private sealed record MoveCandidate(Move Move, IMoveResult Result)
    {
        public int HeuristicScore => Result.PiecesCaptured.Sum(p => p.Value);
    }
}
