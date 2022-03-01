using System.Collections.Immutable;

using DetAct.Interpreter;

namespace DetAct.Behaviour
{
    public interface IBtmlLowLevelBehaviour
    {
        public ImmutableList<FunctionCall> Functions { get; set; }
    }
}
