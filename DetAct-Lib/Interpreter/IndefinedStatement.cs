using Pidgin;

namespace DetAct.Interpreter
{
    public class IndefinedStatement : Statement
    {
        public string Content { get; }

        public SourcePos StartPosition { get; }
        public SourcePos EndPosition { get; }

        public IndefinedStatement(SourcePos startPosition, SourcePos endPosition, string content) : base("")
            => (StartPosition, EndPosition, Content) = (startPosition, endPosition, string.IsNullOrWhiteSpace(content) ? "" : content);

        public override string ToString()
            => $"At ({StartPosition.Line}, {StartPosition.Col}) -> {Content}";
    }
}
