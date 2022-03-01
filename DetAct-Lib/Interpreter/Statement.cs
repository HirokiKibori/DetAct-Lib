namespace DetAct.Interpreter
{
    public abstract class Statement
    {
        public static readonly Statement Default = new DefaultStatement();

        public string Name { get; }

        public Statement(string name) => Name = name;

        private class DefaultStatement : Statement
        {
            public DefaultStatement() : base("DEFAULT") { }
        }
    }

}
