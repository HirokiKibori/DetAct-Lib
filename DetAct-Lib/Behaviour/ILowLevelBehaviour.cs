namespace DetAct.Behaviour
{
    public interface ILowLevelBehaviour : IBehaviour
    {
        public BehaviourTree ParentTree { get; set; }
    }
}
