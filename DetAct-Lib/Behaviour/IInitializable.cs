namespace DetAct.Behaviour
{
    public interface IInitializable
    {
        public void OnInitialize();

        public void OnTerminate(BehaviourStatus status);
    }
}
