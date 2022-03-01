namespace DetAct.Behaviour
{
    public interface IBuildTreeDirector
    {
        public IBehaviourTreeBuilder TreeBuilder { get; }

        public void ChangeTreeBuilder(IBehaviourTreeBuilder treeBuilder);

        public IBehaviourTreeBuilder Construct();
    }
}
