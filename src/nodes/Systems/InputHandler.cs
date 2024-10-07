using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Objects;
using HalfNibbleGame.Objects.Cards;

namespace HalfNibbleGame.Systems;

public sealed class InputHandler
{
    private readonly GameLoop gameLoop;

    private Cursor? cursor;
    private ISelectedThing? selectedPiece;
    private bool isSelectionLocked;
    private bool isActive;

    public InputHandler(GameLoop gameLoop)
    {
        this.gameLoop = gameLoop;
    }

    public void Reset()
    {
        cursor?.Reset();
        selectedPiece = null;
        isSelectionLocked = false;
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

    public void SetTargetSelection(Board board, TargetSelection targeting)
    {
        if (selectedPiece is not null) deselectPiece(board);

        selectedPiece = new SelectedTargeting(board, targeting);
        selectedPiece.HighlightTargetTiles();
    }

    private void handleTileHover(Board board, Tile tile)
    {
        if (!isActive) return;

        if (selectedPiece is not null)
        {
            selectedPiece.EndMovePreview();
            selectedPiece.TryShowMovePreview(tile);
        }
        else
        {
            board.ResetHighlightedTiles();
            if (tile.Piece is { IsStunned: false } piece)
            {
                foreach (var t in piece.ReachableTiles(tile.Coord, board))
                {
                    board[t.Coord].ShowAction(t.Action);
                }
            }
        }
    }

    private void handleTileClick(Board board, Tile tile)
    {
        if (!isActive) return;

        if (selectedPiece?.TryHandleTileClick(gameLoop, tile) ?? false)
        {
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
        selectedPiece?.OnDeselect();
        isSelectionLocked = false;
        selectedPiece = null;
    }

    private interface ISelectedThing
    {
        void TryShowMovePreview(Tile target);
        void EndMovePreview();
        void OnDeselect();
        void HighlightTargetTiles();
        bool TryHandleTileClick(GameLoop gameLoop, Tile clickedTile);
    }

    private sealed class SelectedPiece : ISelectedThing
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

        public void OnDeselect() { }

        public void HighlightTargetTiles()
        {
            foreach (var t in targetTiles)
            {
                board[t.Coord].ShowAction(t.Action);
            }
        }

        public bool TryHandleTileClick(GameLoop gameLoop, Tile clickedTile)
        {
            if (targetTiles.All(t => t.Coord != clickedTile.Coord)) return false;
            var moveCandidate = board.PreviewMove(piece, tile, clickedTile, previousMovesInTurn);
            if (!moveCandidate.Validate())
            {
                return false;
            }
            gameLoop.SubmitMove(moveCandidate);
            return true;
        }
    }

    private sealed class SelectedTargeting : ISelectedThing
    {
        private readonly Board board;
        private readonly TargetSelection selection;
        private bool completed;

        public SelectedTargeting(Board board, TargetSelection selection)
        {
            this.board = board;
            this.selection = selection;
        }

        public void TryShowMovePreview(Tile target) { }

        public void EndMovePreview() { }

        public void OnDeselect()
        {
            if (!completed)
            {
                selection.Cancel();
            }
        }

        public void HighlightTargetTiles()
        {
            foreach (var t in selection.Tiles)
            {
                board[t.Coord].ShowAction(t.Action);
            }
        }

        public bool TryHandleTileClick(GameLoop gameLoop, Tile clickedTile)
        {
            if (selection.Tiles.All(t => t.Coord != clickedTile.Coord)) return false;
            selection.Consume(clickedTile);
            completed = true;
            return true;
        }
    }
}
