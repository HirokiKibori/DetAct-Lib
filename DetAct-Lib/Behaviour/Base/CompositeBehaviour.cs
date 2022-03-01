using System.Collections.Generic;

namespace DetAct.Behaviour.Base
{
    public abstract class CompositeBehaviour : Behaviour, IHighLevelBehaviour
    {
        private readonly List<IBehaviour> _childs = new();

        public IEnumerable<IBehaviour> Childs {
            get {
                if(_childs.Count < 1) {
                    yield return ActionBehaviour.FAILURE;
                }
                else {
                    foreach(var child in _childs) {
                        yield return child;
                    }
                }
            }
        }

        public CompositeBehaviour(IBehaviour child)
            => AddChild(child);

        public virtual bool AddChild(IBehaviour child)
        {
            if(child is null || _childs.Find(c => object.ReferenceEquals(c, child)) is not null)
                return false;

            _childs.Add(item: child);

            return true;
        }
    }
}
