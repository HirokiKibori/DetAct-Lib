using System.Collections.Generic;

namespace DetAct.Behaviour
{
    public interface IHighLevelBehaviour : IBehaviour
    {
        public IEnumerable<IBehaviour> Childs { get; }

        public bool AddChild(IBehaviour child);
    }
}
