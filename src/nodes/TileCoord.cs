using System;
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

    public int Manhattan(TileCoord other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    public int DiagonalDistance(TileCoord other) => Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));
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
    public static Step Up { get; } = new(0, 1);
    public static Step Left { get; } = new(-1, 0);
    public static Step Down { get; } = new(0, -1);
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
