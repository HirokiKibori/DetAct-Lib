using System;

using DetAct.Behaviour.Base;

namespace DetAct.Behaviour.Concrete
{
    public class FunctionalCondition : ConditionBehaviour
    {
        private FunctionalCondition() : base("") { }

        public FunctionalCondition(string name) : base(name) { }

        public Func<BehaviourStatus> Function { get; set; } =
            () => BehaviourStatus.FAILURE;

        protected override BehaviourStatus OnUpdate()
            => Function();
    }
}
