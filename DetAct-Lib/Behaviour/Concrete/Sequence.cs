using System.Collections.Generic;

using DetAct.Behaviour.Base;

namespace DetAct.Behaviour.Concrete
{
    public class Sequence : CompositeBehaviour, IInterruptible, IInitializable
    {
        protected IEnumerator<IBehaviour> enumerator;

        private Sequence() : base(null) { }

        public Sequence(IBehaviour child) : base(child) { }

        public virtual void OnInitialize()
        {
            enumerator = Childs.GetEnumerator();
            enumerator.MoveNext();

            Status = BehaviourStatus.RUNNING;
        }

        public virtual void OnTerminate(BehaviourStatus status)
            => enumerator.Dispose();

        protected override BehaviourStatus OnUpdate()
        {
            do {
                var child = enumerator.Current;
                var result = child.Tick();

                if(result is not BehaviourStatus.SUCCESS)
                    return result;

            } while(enumerator.MoveNext());

            return BehaviourStatus.SUCCESS;
        }

        public virtual void Interrupt()
            => OnTerminate(status: BehaviourStatus.INTERRUPTED);
    }
}
