using System.Threading.Tasks;

namespace HalfNibbleGame.Systems;

public sealed record MoveOverride(Task Animation, MoveOverride.Do Execute)
{
    public delegate void Do(Move move, IMoveSideEffects sideEffects);
}
