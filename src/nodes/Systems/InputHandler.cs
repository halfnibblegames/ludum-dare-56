using Godot;
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
        if (selectedPiece?.TryHandleTileClick(tile) is { } move)
        {
            move.Execute();
            selectedPiece = null;
            return;
        }

        if (tile.Piece is not null)
        {
            selectedPiece = new SelectedTile(board, tile, tile.Piece);
        }
    }

    private sealed class SelectedTile
    {
        private readonly Board board;
        private readonly Tile tile;
        private readonly Piece piece;

        public SelectedTile(Board board, Tile tile, Piece piece)
        {
            this.board = board;
            this.tile = tile;
            this.piece = piece;
        }

        public IMove? TryHandleTileClick(Tile clickedTile)
        {
            if (clickedTile == tile)
            {
                return null;
            }

            if (!piece.ReachableTiles(tile.Coord, board).Contains(clickedTile.Coord))
            {
                GD.Print($"{clickedTile.Coord} is not reachable, reachable tiles: {string.Join(", ", piece.ReachableTiles(tile.Coord, board))}");
                return null;
            }

            return Moves.MovePiece(piece, tile, clickedTile);
        }
    }
}
