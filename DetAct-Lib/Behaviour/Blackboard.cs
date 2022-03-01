using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DetAct.Behaviour
{
    public class Blackboard<T> : ConcurrentDictionary<string, T>
    {
        public Blackboard() : base() { }

        public Blackboard(IEnumerable<KeyValuePair<string, T>> collection) : base(collection) { }
    }
}
