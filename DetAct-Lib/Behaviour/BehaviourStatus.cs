namespace DetAct.Behaviour
{
    public enum BehaviourStatus : int
    {
        SUCCESS = 0,
        FAILURE = 1,
        RUNNING = 2,

        INDEFINITE = -1,
        INTERRUPTED = -2
    }
}
