namespace DetAct.Interpreter
{
    public interface IValue { }

    public readonly struct StringValue : IValue
    {
        public string Value { get; }

        public StringValue(string value)
            => Value = value;

        public override string ToString()
            => $"\"{Value}\"";
    }

    public readonly struct FieldDescriptor : IValue
    {
        public string Value { get; }

        public FieldDescriptor(string value)
            => Value = value;

        public override string ToString()
            => Value;
    }

    public readonly struct BoardValue : IValue
    {
        public string BoadName { get; }

        public IValue FieldDescriptor { get; }

        public BoardValue(string boardName, IValue fieldDescriptor)
            => (BoadName, FieldDescriptor) = (boardName, fieldDescriptor);

        public override string ToString()
            => $"{BoadName}[{FieldDescriptor}]";
    }
}
