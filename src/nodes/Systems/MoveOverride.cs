using System;
using System.Threading.Tasks;

namespace HalfNibbleGame.Systems;

public sealed record MoveOverride(Func<Task> AnimationFactory, MoveOverride.Do Execute)
{
    public delegate void Do(Move move, IMoveSideEffects sideEffects);
}
