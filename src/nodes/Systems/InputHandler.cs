using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class InputHandler
{
    private readonly GameLoop gameLoop;

    private SelectedPiece? selectedPiece;
    private bool isSelectionLocked;
    private bool isActive;

    public InputHandler(GameLoop gameLoop)
    {
        this.gameLoop = gameLoop;
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    public void SetContinuation(Board board, MoveContinuation continuation)
    {
        selectedPiece = new SelectedPiece(
            board, continuation.From, continuation.Piece, continuation.TargetTiles, continuation.PreviousMovesInTurn);
        selectedPiece.HighlightTargetTiles();
        isSelectionLocked = true;
    }

    public void HandleTileClick(Board board, Tile tile)
    {
        if (!isActive) return;

        if (selectedPiece?.TryHandleTileClick(tile) is { } move)
        {
            gameLoop.SubmitMove(move);
            deselectPiece(board);
            return;
        }

        if (isSelectionLocked || tile.Piece is not { IsEnemy: false, IsStunned: false } piece) return;
        if (selectedPiece is not null)
        {
            deselectPiece(board);
        }
        selectPiece(board, tile, piece);
    }

    private void selectPiece(Board board, Tile tile, Piece piece)
    {
        selectedPiece = new SelectedPiece(board, tile, piece);
        selectedPiece.HighlightTargetTiles();
    }

    private void deselectPiece(Board board)
    {
        board.ResetHighlightedTiles();
        isSelectionLocked = false;
        selectedPiece = null;
    }

    private sealed class SelectedPiece
    {
        private readonly Board board;
        private readonly Tile tile;
        private readonly Piece piece;
        private readonly IReadOnlyList<TileCoord> targetTiles;
        private readonly int previousMovesInTurn;

        public SelectedPiece(Board board, Tile tile, Piece piece)
            : this(board, tile, piece, piece.ReachableTiles(tile.Coord, board), 0) { }

        public SelectedPiece(
            Board board, Tile tile, Piece piece, IEnumerable<TileCoord> targetTiles, int previousMovesInTurn)
        {
            this.board = board;
            this.tile = tile;
            this.piece = piece;
            this.targetTiles = targetTiles.ToList();
            this.previousMovesInTurn = previousMovesInTurn;
        }

        public void HighlightTargetTiles()
        {
            foreach (var t in targetTiles)
            {
                board[t].Highlight();
            }
        }

        public Move? TryHandleTileClick(Tile clickedTile)
        {
            if (!targetTiles.Contains(clickedTile.Coord)) return null;
            var moveCandidate = board.PreviewMove(piece, tile, clickedTile, previousMovesInTurn);
            return moveCandidate.Validate() ? moveCandidate : null;
        }
    }
}
