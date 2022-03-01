namespace DetAct.Behaviour.Base
{
    public abstract class ActionBehaviour : Behaviour, ILowLevelBehaviour
    {
        public BehaviourTree ParentTree { get; set; }

        public ActionBehaviour(string name)
            => Name = name;

        /// <summary>
        /// Default-action with <i>BehaviourStatus.SUCCESS</i> as update-result.
        /// </summary>
        public static readonly ActionBehaviour SUCCESS = new DefaultAction("SUCCESS", status: BehaviourStatus.SUCCESS);
        /// <summary>
        /// Default-action with <i>BehaviourStatus.FAILURE</i> as update-result.
        /// </summary>
        public static readonly ActionBehaviour FAILURE = new DefaultAction("FAILURE", status: BehaviourStatus.FAILURE);

        /// <summary>
        /// Default-action with <i>BehaviourStatus.RUNNING</i> as update-result.
        /// </summary>
        public static readonly ActionBehaviour RUNNING = new DefaultAction("RUNNING", status: BehaviourStatus.RUNNING);
        /// <summary>
        /// Default-action with <i>BehaviourStatus.INTERRUPTED</i> as update-result.
        /// </summary>
        public static readonly ActionBehaviour INTERRUPTED = new DefaultAction("INTERRUPTED", status: BehaviourStatus.INTERRUPTED);

        /// <summary>
        /// Default-action with <i>BehaviourStatus.INDEFINITE</i> as update-result.
        /// </summary>
        public static readonly ActionBehaviour INDEFINITE = new DefaultAction("INDEFINITE", status: BehaviourStatus.INDEFINITE);

        private class DefaultAction : ActionBehaviour
        {
            public DefaultAction(string name, BehaviourStatus status) : base(name)
                => Status = status;

            protected override BehaviourStatus OnUpdate()
                => Status;
        }
    }
}
