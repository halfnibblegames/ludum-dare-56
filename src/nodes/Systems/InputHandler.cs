using System.Linq;
using Godot;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed class InputHandler
{
    private readonly Board board;

    private SelectedPiece? selectedPiece;

    public InputHandler(Board board)
    {
        this.board = board;
    }

    public void HandleTileClick(Tile tile)
    {
        if (selectedPiece?.TryHandleTileClick(tile) is { } move)
        {
            move.Execute();
            deselectPiece();
            return;
        }

        if (tile.Piece is not { } piece) return;
        if (selectedPiece is not null)
        {
            deselectPiece();
        }
        selectPiece(tile, piece);
    }

    private void selectPiece(Tile tile, Piece piece)
    {
        selectedPiece = new SelectedPiece(board, tile, piece);
        selectedPiece.HighlightReachableTiles();
    }

    private void deselectPiece()
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

        public IMove? TryHandleTileClick(Tile clickedTile)
        {
            if (clickedTile == tile)
            {
                return null;
            }

            if (!piece.ReachableTiles(tile.Coord, board).ToHashSet().Contains(clickedTile.Coord))
            {
                GD.Print($"{clickedTile.Coord} is not reachable, reachable tiles: {string.Join(", ", piece.ReachableTiles(tile.Coord, board))}");
                return null;
            }

            return Moves.MovePiece(piece, tile, clickedTile);
        }
    }
}
