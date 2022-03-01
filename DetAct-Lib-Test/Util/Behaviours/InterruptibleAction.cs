using DetAct.Behaviour;
using DetAct.Behaviour.Base;

namespace DetAct.Test.Util.Behaviours
{
    public class InterruptibleAction : ActionBehaviour, IInterruptible, IInitializable
    {
        private bool isRunning = false;

        private bool interrupted = false;

        public InterruptibleAction(string name) : base(name) { }

        public void OnInitialize() => isRunning = true;

        public void OnTerminate(BehaviourStatus status) => interrupted = false;

        public void Terminate() => isRunning = false;

        protected override BehaviourStatus OnUpdate() => isRunning ? BehaviourStatus.RUNNING
            : (interrupted ? BehaviourStatus.FAILURE : BehaviourStatus.SUCCESS);

        public void Interrupt()
        {
            isRunning = false;
            interrupted = true;
        }
    }
}
