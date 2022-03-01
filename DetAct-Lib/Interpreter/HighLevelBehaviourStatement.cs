using System.Collections.Generic;
using System.Collections.Immutable;

namespace DetAct.Interpreter
{
    public class HighLevelBehaviourStatement : BehaviourStatement
    {
        public ImmutableList<string> Childs { get; }

        public HighLevelBehaviourStatement(string name, string type, IEnumerable<string> childs, IEnumerable<string> parameters) : base(name, type, parameters)
            => Childs = childs.ToImmutableList();

        private string ParametersToString()
            => Parameters.IsEmpty ? "" : $"({string.Join(", ", Parameters)})";

        public override string ToString()
            => $"{Name}:{Type}{ParametersToString()} := [ {string.Join(" | ", Childs)} ]";
    }
}
