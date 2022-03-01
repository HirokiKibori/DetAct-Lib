using DetAct.Behaviour;

namespace DetAct.Test.Util.Behaviours
{
    public class TestBehaviour : DetAct.Behaviour.Behaviour, IInitializable, IInterruptible
    {
        public new BehaviourStatus Status { get => base.Status; set => base.Status = value; }

        public bool IsInitialized { get; private set; } = false;
        public bool IsTerminated { get; private set; } = false;
        public bool IsInterrupted { get; private set; } = false;

        public void OnInitialize() => IsInitialized = true;

        public void OnTerminate(BehaviourStatus status)
        {
            IsTerminated = true;
            Status = BehaviourStatus.INDEFINITE;
        }

        public void Interrupt() => IsInterrupted = true;

        protected override BehaviourStatus OnUpdate() => BehaviourStatus.RUNNING;
    }
}
