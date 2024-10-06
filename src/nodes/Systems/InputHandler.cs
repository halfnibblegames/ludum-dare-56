using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class InputHandler
{
    private readonly GameLoop gameLoop;

    private Cursor? cursor;
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
        if (cursor is null)
        {
            cursor = Global.Services.Get<Cursor>();
            cursor.TileHovered += handleTileHover;
            cursor.TileClicked += handleTileClick;
        }

        cursor.Activate();
    }

    public void Deactivate()
    {
        isActive = false;
        cursor?.Deactivate();
    }

    public void SetContinuation(Board board, MoveContinuation continuation)
    {
        selectedPiece = new SelectedPiece(
            board, continuation.From, continuation.Piece, continuation.TargetTiles, continuation.PreviousMovesInTurn);
        selectedPiece.HighlightTargetTiles();
        isSelectionLocked = true;
    }

    private void handleTileHover(Board board, Tile tile)
    {
        if (!isActive) return;

        selectedPiece?.EndMovePreview();
        selectedPiece?.TryShowMovePreview(tile);
    }

    private void handleTileClick(Board board, Tile tile)
    {
        if (!isActive) return;

        if (selectedPiece?.TryHandleTileClick(tile) is { } move)
        {
            gameLoop.SubmitMove(move);
            deselectPiece(board);
            return;
        }

        if (isSelectionLocked) return;
        if (selectedPiece is not null)
        {
            deselectPiece(board);
        }

        if (tile.Piece is { IsEnemy: false, IsStunned: false } piece)
        {
            selectPiece(board, tile, piece);
        }
    }

    private void selectPiece(Board board, Tile tile, Piece piece)
    {
        selectedPiece = new SelectedPiece(board, tile, piece);
        selectedPiece.HighlightTargetTiles();
    }

    private void deselectPiece(Board board)
    {
        board.ResetHighlightedTiles();
        selectedPiece?.EndMovePreview();
        isSelectionLocked = false;
        selectedPiece = null;
    }

    private sealed class SelectedPiece
    {
        private readonly Board board;
        private readonly Tile tile;
        private readonly Piece piece;
        private readonly IReadOnlyList<ReachableTile> targetTiles;
        private readonly int previousMovesInTurn;

        public SelectedPiece(Board board, Tile tile, Piece piece)
            : this(board, tile, piece, piece.ReachableTiles(tile.Coord, board), 0) { }

        public SelectedPiece(
            Board board, Tile tile, Piece piece, IEnumerable<ReachableTile> targetTiles, int previousMovesInTurn)
        {
            this.board = board;
            this.tile = tile;
            this.piece = piece;
            this.targetTiles = targetTiles.ToList();
            this.previousMovesInTurn = previousMovesInTurn;
        }

        public void TryShowMovePreview(Tile target)
        {
            var moveCandidate = board.PreviewMove(piece, tile, target, previousMovesInTurn);
            if (moveCandidate.Validate())
            {
                piece.SpawnMovePreview(moveCandidate);
            }
        }

        public void EndMovePreview()
        {
            piece.EndMovePreview();
        }

        public void HighlightTargetTiles()
        {
            foreach (var t in targetTiles)
            {
                board[t.Coord].ShowAction(t.Action);
            }
        }

        public Move? TryHandleTileClick(Tile clickedTile)
        {
            if (targetTiles.All(t => t.Coord != clickedTile.Coord)) return null;
            var moveCandidate = board.PreviewMove(piece, tile, clickedTile, previousMovesInTurn);
            return moveCandidate.Validate() ? moveCandidate : null;
        }
    }
}
