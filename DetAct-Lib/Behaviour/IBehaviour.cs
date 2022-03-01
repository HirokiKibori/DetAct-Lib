using System.Collections.Generic;

namespace DetAct.Behaviour
{
    public interface IBehaviour
    {
        public string Name { get; set; }

        public BehaviourStatus Status { get; }

        public BehaviourStatus Tick();

        public void ResetStatus();

        public List<string> GetBehaviourInfo();

        public void ProcessParameterList(string[] values);
    }
}
