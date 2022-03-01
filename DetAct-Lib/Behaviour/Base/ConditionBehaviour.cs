namespace DetAct.Behaviour.Base
{
    public abstract class ConditionBehaviour : Behaviour, ILowLevelBehaviour
    {
        public BehaviourTree ParentTree { get; set; }

        public ConditionBehaviour(string name)
            => Name = name;

        /// <summary>
        /// Default-condition with <i>BehaviourStatus.SUCCESS</i> as update-result.
        /// </summary>
        public static readonly ConditionBehaviour SUCCESS = new DefaultCondition("SUCCESS", status: BehaviourStatus.SUCCESS);
        /// <summary>
        /// Default-condition with <i>BehaviourStatus.FAILURE</i> as update-result.
        /// </summary>
        public static readonly ConditionBehaviour FAILURE = new DefaultCondition("FAILURE", status: BehaviourStatus.FAILURE);

        /// <summary>
        /// Default-condition with <i>BehaviourStatus.INDEFINITE</i> as update-result.
        /// </summary>
        public static readonly ConditionBehaviour INDEFINITE = new DefaultCondition("INDEFINITE", status: BehaviourStatus.INDEFINITE);

        private class DefaultCondition : ConditionBehaviour
        {
            public DefaultCondition(string name, BehaviourStatus status) : base(name)
                => Status = status;

            protected override BehaviourStatus OnUpdate()
                => Status;
        }
    }
}
