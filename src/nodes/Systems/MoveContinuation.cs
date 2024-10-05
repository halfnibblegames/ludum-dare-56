using System.Collections.Generic;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed record MoveContinuation(
    Tile From, Piece Piece, IReadOnlyList<TileCoord> TargetTiles, int PreviousMovesInTurn);
