using Godot;
using HalfNibbleGame.Objects.Cards;

namespace HalfNibbleGame.Systems;

public sealed record Choice(PackedScene PiecePrefab, Card Card);
