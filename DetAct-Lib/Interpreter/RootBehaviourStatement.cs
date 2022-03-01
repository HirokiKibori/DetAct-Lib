using System;

namespace DetAct.Interpreter
{
    public class RootBehaviourStatement : HighLevelBehaviourStatement
    {
        public RootBehaviourStatement(string name, string child)
            : base(name, "Root", new[] { child }, Array.Empty<string>()) { }
    }
}
