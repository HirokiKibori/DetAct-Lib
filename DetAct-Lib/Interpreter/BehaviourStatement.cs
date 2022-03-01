using System.Collections.Generic;
using System.Collections.Immutable;

namespace DetAct.Interpreter
{
    public class BehaviourStatement : Statement
    {
        public string Type { get; }

        public ImmutableArray<string> Parameters { get; }

        public BehaviourStatement(string name, string type, IEnumerable<string> parameters) : base(name)
            => (Type, Parameters) = (type, parameters.ToImmutableArray());
    }
}
