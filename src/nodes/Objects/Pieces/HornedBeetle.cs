﻿using System;
using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class HornedBeetle : Piece
{
    public override int Value => 4;

    public override string Name => "Horned Beetle";

    public override string GetHelpText() =>
        $"* Moves horizontal or vertically.\n* Captures all pieces in their path.\n* {(IsStunned?$"[b]Stunned for the next {StunnedTurnsLeft+1} turns![/b]":"Capturing makes them Stunned for 2 turns.")}";

    private static readonly Step[] validSteps = { Step.Up, Step.Left, Step.Right, Step.Down };

    public override IEnumerable<TileCoord> ReachableTiles(TileCoord currentTile, Board board) =>
        validSteps.SelectMany(step =>
            currentTile.EnumerateDirection(step)
                .TakeWhile(c => !ContainsSameColorPiece(board[c])));

    public override void OnMove(Move move, IMoveSideEffects sideEffects)
    {
        var diff = move.To.Coord - move.From.Coord;
        var singleStep = new Step(Math.Sign(diff.X), Math.Sign(diff.Y));
        var traversedTiles = move.From.Coord.EnumerateDirection(singleStep).TakeWhileIncluding(t => t != move.To.Coord);

        var captured = false;
        foreach (var traversedTile in traversedTiles)
        {
            var boardTile = move.Board[traversedTile];
            if (boardTile.Piece is { } piece && piece.IsEnemy != IsEnemy)
            {
                sideEffects.CapturePiece(boardTile);
                captured = true;
            }
        }

        if (captured)
        {
            sideEffects.Stun(2);
        }
    }
}
