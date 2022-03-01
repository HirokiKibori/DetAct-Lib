using System;
using System.Collections.Generic;

using DetAct.Behaviour.Base;

namespace DetAct.Behaviour.Concrete
{
    public class Conditional : Sequence
    {
        public ConditionBehaviour Condition { get; private set; }

        public new IEnumerable<IBehaviour> Childs {
            get {
                yield return Condition;

                foreach(var child in base.Childs)
                    yield return child;
            }
        }

        private Conditional() : base(null) { }

        public Conditional(ConditionBehaviour condition, IBehaviour child) : base(null)
        {
            AddChild(condition);
            AddChild(child);
        }

        public override bool AddChild(IBehaviour child)
        {
            if(child is null)
                return false;

            if(Condition is null) {
                if(child is not ConditionBehaviour buffer)
                    throw new ArgumentException(message: "The first child in a 'Conditional' needs to be a 'ConditionBehaviour'.", paramName: nameof(child));

                return ChangeCondition(buffer);
            }

            return base.AddChild(child);
        }

        public bool ChangeCondition(ConditionBehaviour condition)
        {
            if(condition is null)
                return false;

            Condition = condition;

            return true;
        }

        protected override BehaviourStatus OnUpdate()
        {
            if(Condition.Tick() is not BehaviourStatus.SUCCESS) {
                InterruptBehaviour(Behaviour: this);

                return BehaviourStatus.FAILURE;
            }

            return base.OnUpdate();
        }
    }
}
