using System;
using System.Collections.Generic;
using System.Linq;

using DetAct.Behaviour.Base;

namespace DetAct.Behaviour.Concrete
{
    public class Repeater : DecoratorBehaviour, IInitializable
    {
        private int currentRepetation;

        public int Repetitions { get; set; }

        private Repeater() : base(null)
            => Repetitions = 1;

        public Repeater(IBehaviour child, int repetitions = 1) : base(child)
            => Repetitions = repetitions;

        public override void ProcessParameterList(string[] values)
        {
            if(values == null || values.Length < 1)
                return;

            if(values.Length > 1)
                throw new ArgumentException($"{Name}:Repeater parameter-list length is greather than 1. Only one or zero values are accepted.");

            if(string.IsNullOrWhiteSpace(values[0]))
                return;

            if(int.TryParse(values[0], out int result)) {
                Repetitions = result;

                return;
            }

            throw new ArgumentException(message: $"{Name}:Repeater parameter '{values[0]}' can not converted to an integral value.", paramName: nameof(values));
        }

        public override List<string> GetBehaviourInfo()
        {
            var result = base.GetBehaviourInfo();

            result.Add($"Max Repetitions = {Repetitions}");
            result.Add($"Repetition = {currentRepetation}");

            return result;
        }

        public void OnInitialize()
            => currentRepetation = 0;

        public void OnTerminate(BehaviourStatus status) { }

        protected override BehaviourStatus OnUpdate()
        {
            while(true) {
                var child = Childs.Single();
                child.Tick();

                if(child.Status is BehaviourStatus.RUNNING)
                    break;

                if(child.Status is BehaviourStatus.FAILURE)
                    return BehaviourStatus.FAILURE;

                if(Repetitions > 0 && ++currentRepetation >= Repetitions)
                    return BehaviourStatus.SUCCESS;

                InterruptBehaviour(child);
            }

            return BehaviourStatus.RUNNING;
        }
    }
}
