using System;
using System.Collections.Generic;
using System.Linq;
using HalfNibbleGame.Objects;

namespace HalfNibbleGame;

public readonly struct TileCoord : IEquatable<TileCoord>
{
    public int X { get; }
    public int Y { get; }

    public TileCoord(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool IsValid() => X >= 0 && Y >= 0 && X < Board.Width && Y < Board.Height;

    public IEnumerable<TileCoord> EnumerateValidAdjacent() => EnumerateAdjacent().Where(t => t.IsValid());

    public IEnumerable<TileCoord> EnumerateAdjacent()
    {
        yield return this + Step.Right;
        yield return this + Step.Up;
        yield return this + Step.Left;
        yield return this + Step.Down;
    }

    public IEnumerable<TileCoord> EnumerateValidNeighboring() => EnumerateNeighboring().Where(t => t.IsValid());

    public IEnumerable<TileCoord> EnumerateNeighboring()
    {
        yield return this + Step.Right;
        yield return this + Step.UpRight;
        yield return this + Step.Up;
        yield return this + Step.UpLeft;
        yield return this + Step.Left;
        yield return this + Step.DownLeft;
        yield return this + Step.Down;
        yield return this + Step.DownRight;
    }

    public IEnumerable<TileCoord> EnumerateStepsWhileValid(Step step)
    {
        var t = this + step;
        while (t.IsValid())
        {
            yield return t;
            t += step;
        }
    }

    public static TileCoord operator +(TileCoord coord, Step step) => new(coord.X + step.X, coord.Y + step.Y);

    public bool Equals(TileCoord other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is TileCoord other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (X * 397) ^ Y;
        }
    }

    public override string ToString() => $"({X}, {Y})";
}

public readonly struct Step
{
    public int X { get; }
    public int Y { get; }

    public Step(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public static Step Right { get; } = new(1, 0);
    public static Step Up { get; } = new(0, -1);
    public static Step Left { get; } = new(-1, 0);
    public static Step Down { get; } = new(0, 1);
    public static Step UpRight { get; } = Up + Right;
    public static Step UpLeft { get; } = Up + Left;
    public static Step DownLeft { get; } = Down + Left;
    public static Step DownRight { get; } = Down + Right;

    public static Step operator -(Step step) => new(-step.X, -step.Y);
    public static Step operator +(Step left, Step right) => new(left.X + right.X, left.Y + right.Y);
    public static Step operator -(Step left, Step right) => new(left.X - right.X, left.Y - right.Y);
    public void Deconstruct(out int X, out int Y)
    {
        X = this.X;
        Y = this.Y;
    }
}
