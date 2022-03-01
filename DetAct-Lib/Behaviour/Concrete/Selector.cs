using System.Collections.Generic;

using DetAct.Behaviour.Base;

namespace DetAct.Behaviour.Concrete
{
    public class Selector : CompositeBehaviour, IInterruptible, IInitializable
    {
        private IEnumerator<IBehaviour> enumerator;

        private Selector() : base(null) { }

        public Selector(IBehaviour child) : base(child) { }

        public void OnInitialize()
        {
            enumerator = Childs.GetEnumerator();
            enumerator.MoveNext();

            Status = BehaviourStatus.RUNNING;
        }

        public void OnTerminate(BehaviourStatus status)
            => enumerator.Dispose();

        protected override BehaviourStatus OnUpdate()
        {
            do {
                var child = enumerator.Current;
                var result = child.Tick();

                if(result is not BehaviourStatus.FAILURE)
                    return result;
            }
            while(enumerator.MoveNext());

            return BehaviourStatus.FAILURE;
        }

        public void Interrupt()
            => OnTerminate(status: BehaviourStatus.INTERRUPTED);
    }
}
