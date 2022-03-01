using System;
using System.Collections.Generic;
using System.Linq;

using DetAct.Behaviour.Base;

namespace DetAct.Behaviour.Concrete
{
    public class ParallelBehaviour : CompositeBehaviour, IInterruptible, IInitializable
    {
        private readonly List<ValueTuple<BehaviourStatus, IBehaviour>> statusList = new();

        private Policy successPolicy = Policy.ONE;

        private Policy failurePolicy = Policy.ONE;

        private ParallelBehaviour() : base(null) { }

        public ParallelBehaviour(IBehaviour child, Policy successPolicy, Policy failurePolicy) : base(child)
            => (this.successPolicy, this.failurePolicy) = (successPolicy, failurePolicy);

        public override void ProcessParameterList(string[] values)
        {
            if(values.Length != 2)
                throw new ArgumentException(message: "A 'ParallelBehaviour' needs exactly two Policies as parameters.");

            successPolicy = GetPolicy(values[0].Trim());
            failurePolicy = GetPolicy(values[1].Trim());
        }

        private static Policy GetPolicy(string policy)
        {
            if(policy == "1" || policy.ToLower() == "one")
                return Policy.ONE;

            if(policy.ToLower() == "n" || policy.ToLower() == "all")
                return Policy.ALL;

            throw new ArgumentException(message: "Wrong policy. A policy can be '1' or 'ONE' for 'One'. A policy for 'All' can be 'ALL' or 'N'.", paramName: nameof(policy));
        }

        public override List<string> GetBehaviourInfo()
        {
            var result = base.GetBehaviourInfo();

            result.Add($"Success Policy = {successPolicy}");
            result.Add($"Failure Policy = {failurePolicy}");

            return result;
        }

        public void OnInitialize()
        {
            foreach(var child in Childs) {
                statusList.Add(item: new ValueTuple<BehaviourStatus, IBehaviour>(BehaviourStatus.INDEFINITE, child));
            }
        }

        public void OnTerminate(BehaviourStatus status)
        {
            if(status is not BehaviourStatus.INTERRUPTED)
                InterruptBehaviour(Behaviour: this);

            statusList.Clear();
        }

        protected override BehaviourStatus OnUpdate()
        {
            var childCount = Childs.Count();

            statusList.FindAll(item => item.Item1 is BehaviourStatus.INDEFINITE)
                .ForEach(item => item.Item1 = item.Item2.Tick());

            statusList.FindAll(item => item.Item1 is BehaviourStatus.RUNNING)
                .ForEach(item => item.Item1 = item.Item2.Status);

            int successCount = statusList.FindAll(item => item.Item1 is BehaviourStatus.SUCCESS).Count;
            int failureCount = statusList.FindAll(item => item.Item1 is BehaviourStatus.FAILURE).Count;

            if((failurePolicy is Policy.ONE && failureCount >= 1) ||
               (failurePolicy is Policy.ALL && failureCount == childCount))
                return BehaviourStatus.FAILURE;

            if((successPolicy is Policy.ONE && successCount >= 1) ||
               (successPolicy is Policy.ALL && successCount == childCount))
                return BehaviourStatus.SUCCESS;

            // If successPolicy and failurePolicy are both Policy.All and both counts > 0,
            // the Behaviour fails in the case both counts are equal to the list-size, too.
            if(failureCount + successCount == childCount)
                return BehaviourStatus.FAILURE;

            return BehaviourStatus.RUNNING;
        }

        public void Interrupt()
            => OnTerminate(status: BehaviourStatus.INTERRUPTED);
    }
}
