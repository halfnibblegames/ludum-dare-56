using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Objects;
using HalfNibbleGame.Objects.Pieces;

namespace HalfNibbleGame.Systems;

sealed class EnemyBrain
{
    // Heuristic weights
    private const int captureWeight = 100;
    private const int threatenedWeight = 40;
    private const int approachQueenWeight = 1;

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
        if (pieces.Count == 0) return Enumerable.Empty<Move>();

        // 25% chance of moving two pieces.
        var numberOfPieces = random.NextDouble() < 0.25 ? 2 : 1;

        return planMovesForPieces(pieces, numberOfPieces);
    }

    public IEnumerable<Move> ImproveMoves(IEnumerable<Move> moves)
    {
        GD.Print("Improving moves for pieces");
        var movesAsList = moves.ToList();
        var validPieces = movesAsList
            .Where(m =>
            {
                if (m.Piece.IsDead) return false;

                var expectedTile = m.From;
                var expectedPiece = m.Piece;
                return expectedPiece == expectedTile.Piece;
            })
            .Select(m => PlacedPiece.FromTile(m.Board, m.From))
            .ToList();

        GD.Print($"We found {movesAsList.Count} moves and {validPieces.Count} valid pieces left");

        return planMovesForPieces(validPieces, validPieces.Count);
    }

    private IEnumerable<Move> planMovesForPieces(IReadOnlyList<PlacedPiece> placedPieces, int pieceLimit)
    {
        var ctx = makeContext();

        var bestMovePerPiece = placedPieces.ToDictionary(
            piece => piece,
            piece =>
            {
                var moves = enumerateMoves(piece);
                var bestMove =
                    moves.OrderByDescending(m => m.HeuristicScore(ctx)).FirstOrDefault();
                return bestMove;
            });

        var orderedPieces = placedPieces
            .Where(p => bestMovePerPiece[p] is not null)
            .OrderByDescending(p => bestMovePerPiece[p]!.HeuristicScore(ctx))
            .ToList();

        var occupiedTiles = new List<TileCoord>();
        foreach (var piece in orderedPieces)
        {
            var candidateMove = bestMovePerPiece[piece]!;

            // Avoid multiple pieces trying to move to the same other tile
            if (occupiedTiles.Contains(candidateMove.Move.To.Coord)) continue;

            yield return candidateMove.Move;
            occupiedTiles.Add(candidateMove.Move.To.Coord);

            pieceLimit--;
            if (pieceLimit == 0) break;
        }
    }

    private MoveContext makeContext()
    {
        var queenBeePos = board.Tiles
                .FirstOrDefault(t => t.Piece is QueenBee { IsEnemy: false })?.Coord ??
            new TileCoord(4, 0); // Default to the bottom center
        var threatenedTiles = board.Tiles
            .Where(t => t.Piece is { IsEnemy: false })
            .SelectMany(t => t.Piece!.ReachableTiles(t.Coord, board))
            .Select(rt => rt.Coord)
            .ToHashSet();

        return new MoveContext(queenBeePos, threatenedTiles);
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
        GD.Print($"Enumerating all moves from {piece.Piece.DisplayName}");
        foreach (var t in piece.ReachableTiles)
        {
            var move = board.PreviewMove(piece.Piece, piece.Tile, board[t.Coord], 0);
            var result = move.Preview();
            yield return new MoveCandidate(move, result);
        }
    }

    private sealed record PlacedPiece(Piece Piece, Tile Tile, IReadOnlyList<ReachableTile> ReachableTiles)
    {
        public static PlacedPiece FromTile(Board board, Tile tile) =>
            new(tile.Piece!, tile, tile.Piece!.ReachableTiles(tile.Coord, board).ToList().AsReadOnly());
    }

    private sealed record MoveCandidate(Move Move, IMoveResult Result)
    {
        public int HeuristicScore(MoveContext context) =>
            captureWeight * Result.PiecesCaptured.Sum(p => p.Value) +
            threatenedWeight * threatenedOccupationImprovement(context.ThreatenedTiles) +
            approachQueenWeight * distanceToQueenImprovement(context.QueenBeePos);

        private int threatenedOccupationImprovement(HashSet<TileCoord> threatenedTiles)
        {
            var wasThreatened = threatenedTiles.Contains(Move.From.Coord) ? 1 : 0;
            var isThreatened = threatenedTiles.Contains(Move.To.Coord) ? 1 : 0;

            return (wasThreatened - isThreatened) * Move.Piece.Value * (1 + Result.StunTime);
        }

        private int distanceToQueenImprovement(TileCoord queenBeePos)
        {
            var oldDistance = queenBeePos.Manhattan(Move.From.Coord);
            var newDistance = queenBeePos.Manhattan(Move.To.Coord);

            return oldDistance - newDistance;
        }
    }

    private sealed record MoveContext(TileCoord QueenBeePos, HashSet<TileCoord> ThreatenedTiles);
}
