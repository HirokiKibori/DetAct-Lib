using DetAct.Behaviour.Base;

using System.Linq;

namespace DetAct.Behaviour.Concrete
{
    public class Inverter : DecoratorBehaviour
    {
        private Inverter() : base(null) { }

        public Inverter(IBehaviour child) : base(child) { }

        protected override BehaviourStatus OnUpdate()
        {
            var child = Childs.Single();
            var result = child.Tick();

            if(result is BehaviourStatus.SUCCESS)
                return BehaviourStatus.FAILURE;

            if(result is BehaviourStatus.FAILURE)
                return BehaviourStatus.SUCCESS;

            return result;
        }
    }
}
