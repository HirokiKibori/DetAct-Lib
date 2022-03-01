namespace DetAct.Interpreter
{
    public class Configuration : Statement
    {
        public string Value { get; }

        public Configuration(string name, string value) : base(name)
            => Value = value;

        public override string ToString()
            => $"{Name} := {Value}";
    }
}
