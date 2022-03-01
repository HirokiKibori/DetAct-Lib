using System;

using System.Collections.Generic;
using System.Collections.Immutable;

namespace DetAct.Interpreter
{
    public class LowLevelBehaviourStatement : BehaviourStatement
    {
        public ImmutableList<FunctionCall> Functions { get; }

        public LowLevelBehaviourStatement(string name, string type, ImmutableList<FunctionCall> functions, IEnumerable<string> parameters) : base(name, type, parameters)
            => Functions = functions is null ? Array.Empty<FunctionCall>().ToImmutableList() : functions;

        private string ParametersToString()
            => Parameters.IsEmpty ? "" : $"({string.Join(", ", Parameters)})";

        public override string ToString()
            => $"{Name}:{Type}{ParametersToString()}" + (Functions.IsEmpty ? ";" : $" := [{string.Join(", ", Functions)}];");
    }
}
