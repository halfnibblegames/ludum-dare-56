using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class InputHandler
{
    private readonly Board board;

    private SelectedTile? selectedPiece;

    public InputHandler(Board board)
    {
        this.board = board;
    }

    public void HandleTileClick(Tile tile)
    {
        if (selectedPiece?.TryHandleTileClick(tile) ?? false)
        {
            return;
        }

        if (tile.Piece is not null)
        {
            selectedPiece = new SelectedTile(tile, tile.Piece);
        }
    }

    private sealed class SelectedTile
    {
        private readonly Tile tile;
        private readonly Piece piece;

        public SelectedTile(Tile tile, Piece piece)
        {
            this.tile = tile;
            this.piece = piece;
        }

        public bool TryHandleTileClick(Tile clickedTile)
        {
            if (clickedTile == tile)
            {
                return false;
            }

            Moves.MovePiece(piece, tile, clickedTile).Execute();
            return true;
        }
    }
}
