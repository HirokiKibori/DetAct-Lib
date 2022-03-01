using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using DetAct.Interpreter;

namespace DetAct.Behaviour.Base
{
    public abstract class BtmlCondition : ConditionBehaviour, IBtmlLowLevelBehaviour
    {
        public ImmutableList<FunctionCall> Functions { get; set; } = ImmutableList.Create<FunctionCall>();

        public BtmlCondition(string name) : base(name) { }

        protected override BehaviourStatus OnUpdate()
            => ProcessFunctionCall(Functions.ToDictionary(funcCall => funcCall.Name, funcCall => GetVAluesFromFunctionCall(funcCall.Values)));

        protected IEnumerable<string> GetVAluesFromFunctionCall(IEnumerable<IValue> values)
            => values.Select(value => ParentTree is null ? "null" : GetValueFromFctnValue(value));

        protected virtual string GetValueFromFctnValue(IValue value)
        {
            if(value is BoardValue boardValue) {
                if(!ParentTree.GetBoard(boardValue.BoadName, out var blackBoard))
                    return "null";

                var key = boardValue.FieldDescriptor is BoardValue innerValue
                    ? GetValueFromFctnValue(innerValue)
                    : ((FieldDescriptor)boardValue.FieldDescriptor).Value;

                if(string.IsNullOrEmpty(key) || key.Trim().ToLower() == "null" || !blackBoard.ContainsKey(key))
                    return "null";

                return blackBoard[key];
            }

            if(value is StringValue @string)
                return string.IsNullOrWhiteSpace(@string.Value) ? "null" : @string.Value;

            return "null";
        }

        public override List<string> GetBehaviourInfo()
        {
            var result = base.GetBehaviourInfo();

            if(Functions is null)
                return result;

            foreach(var functionCall in Functions)
                result.Add(functionCall.ToString());

            return result;
        }

        protected abstract BehaviourStatus ProcessFunctionCall(Dictionary<string, IEnumerable<string>> functions);
    }
}
