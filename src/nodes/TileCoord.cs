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

    public bool Equals(TileCoord other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is TileCoord other && Equals(other);
    public override int GetHashCode() => (X * 397) ^ Y;
    public override string ToString() => $"({X}, {Y})";

    public static bool operator ==(TileCoord left, TileCoord right) => left.Equals(right);
    public static bool operator !=(TileCoord left, TileCoord right) => !left.Equals(right);

    public static TileCoord operator +(TileCoord coord, Step step) => new(coord.X + step.X, coord.Y + step.Y);
    public static Step operator -(TileCoord left, TileCoord right) => new(left.X - right.X, left.Y - right.Y);

    public int Manhattan(TileCoord other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    public int DiagonalDistance(TileCoord other) => Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));
}

public readonly struct Step : IEquatable<Step>
{
    public static Step Right { get; } = new(1, 0);
    public static Step Up { get; } = new(0, 1);
    public static Step Left { get; } = new(-1, 0);
    public static Step Down { get; } = new(0, -1);
    public static Step UpRight { get; } = Up + Right;
    public static Step UpLeft { get; } = Up + Left;
    public static Step DownLeft { get; } = Down + Left;
    public static Step DownRight { get; } = Down + Right;

    public int X { get; }
    public int Y { get; }

    public Step(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(Step other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is Step other && Equals(other);
    public override int GetHashCode() => (X * 397) ^ Y;
    public override string ToString() => $"({X:+#;-#;+0}, {Y:+#;-#;+0})";

    public static bool operator ==(Step left, Step right) => left.Equals(right);
    public static bool operator !=(Step left, Step right) => !left.Equals(right);

    public static Step operator -(Step step) => new(-step.X, -step.Y);
    public static Step operator +(Step left, Step right) => new(left.X + right.X, left.Y + right.Y);
    public static Step operator -(Step left, Step right) => new(left.X - right.X, left.Y - right.Y);
    public static Step operator *(int scalar, Step step) => new(scalar * step.X, scalar * step.Y);
    public static Step operator *(Step step, int scalar) => scalar * step;

    public void Deconstruct(out int X, out int Y)
    {
        X = this.X;
        Y = this.Y;
    }
}
