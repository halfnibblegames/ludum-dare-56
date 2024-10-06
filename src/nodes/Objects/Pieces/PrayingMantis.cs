using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Pieces;

public sealed class PrayingMantis : Piece
{
    public override string DisplayName => "Praying Mantis";

    public override string HelpText => "* Moves up to one neighbor space.\n* Captures by swiping three tiles at once.\n* Kind of a bully.";

    public override int Value => 3;

    public override IEnumerable<ReachableTile> ReachableTiles(TileCoord currentTile, Board board) =>
        currentTile.EnumerateAdjacent()
            .Where(c => !ContainsSameColorPiece(board[c]))
            .Select(t => MoveOr(board[t], TileAction.Swipe));

    public override MoveOverride? InterruptMove(Move move)
    {
        if (move.To.Piece is null)
        {
            return null;
        }

        var step = move.To.Coord - move.From.Coord;
        var index = Array.IndexOf(TileCoordExtensions.Steps, step);
        if (index < 0) throw new Exception();

        var stepCount = TileCoordExtensions.Steps.Length;
        var nextStep = TileCoordExtensions.Steps[(index + 1) % stepCount];
        var prevStep = TileCoordExtensions.Steps[(stepCount + index - 1) % stepCount];
        var nextTile = move.Board[move.From.Coord + nextStep];
        var prevTile = move.Board[move.From.Coord + prevStep];

        return new MoveOverride(createAnimation(step), execute);

        void execute(Move m, IMoveSideEffects sideEffects)
        {
            foreach (var tile in new[] { m.To, nextTile, prevTile })
            {
                if (tile.Piece is not null && tile.Piece.IsEnemy != IsEnemy)
                {
                    sideEffects.CapturePiece(tile);
                }
            }
        }
    }

    private Func<Task> createAnimation(Step step)
    {
        var (spriteName, rotation) = lookUpAnimation(step);

        var sprite = GetNode<AnimatedSprite>($"AnimationRoot/{spriteName}");

        return playAnimation;

        async Task playAnimation()
        {
            sprite.RotationDegrees = rotation;
            var anim = new SpriteAnimation(sprite, "default");
            var signal = ToSignal(anim, nameof(SpriteAnimation.Finished));
            AddChild(anim);
            await signal;
        }
    }

    private static (string SpriteName, int rotation) lookUpAnimation(Step step)
    {
        return (step.X, step.Y) switch
        {
            (1, 0) => ("HorizontalSwipe", 90),
            (1, 1) => ("DiagonalSwipe", 0),
            (0, 1) => ("HorizontalSwipe", 0),
            (-1, 1) => ("DiagonalSwipe", -90),
            (-1, 0) => ("HorizontalSwipe", -90),
            (-1, -1) => ("DiagonalSwipe", 180),
            (0, -1) => ("HorizontalSwipe", 180),
            (1, -1) => ("DiagonalSwipe", 90),
            _ => throw new ArgumentOutOfRangeException(nameof(step), step, null)
        };
    }
}
