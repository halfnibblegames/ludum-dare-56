﻿using System.Collections.Generic;
using System.Linq;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class HornedBeetle : Piece
{
    private static readonly Step[] validSteps = { Step.Up, Step.Left, Step.Right, Step.Down };

    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        validSteps.SelectMany(step =>
            currentTile.EnumerateStepsWhileValid(step)
                .TakeWhile(c => !ContainsSameColorPiece(board[c]))
                .TakeWhileIncluding(c => board[c].Piece is null));
}
