using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Systems;

namespace HalfNibbleGame.Objects.Cards;

public abstract class Card : IHelpable
{
    public string DisplayName { get; }
    public abstract Texture GetTexture();

    protected Card(string name)
    {
        DisplayName = name;
    }

    public abstract Task<bool> Use(Board board);

    public abstract string HelpText { get; }
}

public abstract class CardWithTarget : Card
{
    protected CardWithTarget(string name) : base(name) { }

    public sealed override Task<bool> Use(Board board)
    {
        var tcs = new TaskCompletionSource<bool>();
        var ts = new TargetSelection(GetReachableTiles(board).ToList(), useCard, cancel);
        Global.Services.Get<InputHandler>().SetTargetSelection(board, ts);

        return tcs.Task;

        void useCard(Tile tile)
        {
            _ = useCardAsync(tile);
        }

        async Task useCardAsync(Tile tile)
        {
            await Use(board, tile);
            tcs.SetResult(true);
        }

        void cancel()
        {
            tcs.SetResult(false);
        }
    }

    protected abstract IEnumerable<ReachableTile> GetReachableTiles(Board board);

    protected abstract Task Use(Board board, Tile target);
}

public sealed record TargetSelection(IReadOnlyList<ReachableTile> Tiles, Action<Tile> Consume, Action Cancel);
