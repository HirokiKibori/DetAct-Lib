using System;
using System.Collections.Generic;

namespace DetAct.Interpreter
{
    public class FunctionList : Statement
    {
        public IEnumerable<Function> Functions { get; }

        public FunctionList(string name, IEnumerable<Function> functions) : base(name)
            => Functions = functions;

        public override string ToString()
            => $"{Name} -> {Environment.NewLine}- {string.Join("," + Environment.NewLine + "- ", Functions)}";
    }

    public readonly struct Function
    {
        public string Name { get; }

        public IEnumerable<ParameterDefiniton> Parameters { get; }

        public Function(string name, IEnumerable<ParameterDefiniton> parameters)
            => (Name, Parameters) = (name, parameters);

        public override string ToString()
            => $"{Name}({string.Join(", ", Parameters)})";
    }

    public readonly struct FunctionCall
    {
        public string Name { get; }

        public IEnumerable<IValue> Values { get; }

        public FunctionCall(string name, IEnumerable<IValue> values)
            => (Name, Values) = (name, values);

        public override string ToString()
            => $"{Name}({string.Join(", ", Values)})";
    }

    public enum ParameterType
    {
        BOARD,
        STRING,

        INDEFINED
    }

    public readonly struct ParameterDefiniton
    {
        public ParameterType Type { get; }

        public string Name { get; }

        public ParameterDefiniton(string type, string name)
        {
            Name = name;

            Type = type switch
            {
                "string" => ParameterType.STRING,
                "board" => ParameterType.BOARD,
                _ => ParameterType.INDEFINED,
            };
        }

        public override string ToString()
            => $"{Name}:{Type}";
    }
}
