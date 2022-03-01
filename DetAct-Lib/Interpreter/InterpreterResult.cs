using System.Collections.Generic;
using System.Collections.Immutable;

using DetAct.Behaviour;

namespace DetAct.Interpreter
{
    public struct InterpreterResult
    {
        public bool IsValid { get; }

        public BehaviourTree Tree { get; }

        public ImmutableList<InterpreterMessage> Messages { get; }

        public InterpreterResult(bool isValid, BehaviourTree behaviourTree, List<InterpreterMessage> messages)
            => (IsValid, Tree, Messages) = (isValid, behaviourTree, messages.ToImmutableList());
    }
}
