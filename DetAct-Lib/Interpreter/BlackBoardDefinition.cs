using System.Collections.Generic;

namespace DetAct.Interpreter
{
    public class BlackBoardDefinition : Statement
    {
        public Dictionary<string, string> Items { get; }

        public BlackBoardDefinition(string name, IEnumerable<KeyValuePair<string, string>> items) : base(name)
            => Items = new Dictionary<string, string>(items);

        public override string ToString()
            => $"{Name} := [ {string.Join(" , ", Items)} ]";
    }
}
