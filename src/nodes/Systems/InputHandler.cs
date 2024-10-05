using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class InputHandler
{
    private readonly GameLoop gameLoop;

    private SelectedPiece? selectedPiece;
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

    public void HandleTileClick(Board board, Tile tile)
    {
        if (!isActive) return;

        if (selectedPiece?.TryHandleTileClick(tile) is { } move)
        {
            gameLoop.SubmitMove(move);
            deselectPiece(board);
            return;
        }

        if (tile.Piece is not { IsEnemy: false, IsStunned: false } piece) return;
        if (selectedPiece is not null)
        {
            deselectPiece(board);
        }
        selectPiece(board, tile, piece);
    }

    private void selectPiece(Board board, Tile tile, Piece piece)
    {
        selectedPiece = new SelectedPiece(board, tile, piece);
        selectedPiece.HighlightReachableTiles();
    }

    private void deselectPiece(Board board)
    {
        board.ResetHighlightedTiles();
        selectedPiece = null;
    }

    private sealed class SelectedPiece
    {
        private readonly Board board;
        private readonly Tile tile;
        private readonly Piece piece;

        public SelectedPiece(Board board, Tile tile, Piece piece)
        {
            this.board = board;
            this.tile = tile;
            this.piece = piece;
        }

        public void HighlightReachableTiles()
        {
            foreach (var t in piece.ReachableTiles(tile.Coord, board))
            {
                board[t].Highlight();
            }
        }

        public Move? TryHandleTileClick(Tile clickedTile)
        {
            var moveCandidate = board.PreviewMove(piece, tile, clickedTile);
            return moveCandidate.Validate() ? moveCandidate : null;
        }
    }
}
