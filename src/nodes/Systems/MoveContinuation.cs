using System.Collections.Generic;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame.Systems;

public sealed record MoveContinuation(
    Tile From, Piece Piece, IReadOnlyList<ReachableTile> TargetTiles, int PreviousMovesInTurn);
