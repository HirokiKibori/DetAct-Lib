using System;

using DetAct.Behaviour.Base;

namespace DetAct.Behaviour.Concrete
{
    public class FunctionalAction : ActionBehaviour
    {
        private FunctionalAction() : base("") { }

        public FunctionalAction(string name) : base(name) { }

        public Func<BehaviourStatus> Function { get; set; } =
            () => BehaviourStatus.FAILURE;

        protected override BehaviourStatus OnUpdate()
            => Function();
    }
}
