using System.Collections.Generic;

namespace DetAct.Behaviour.Base
{
    public abstract class DecoratorBehaviour : Behaviour, IHighLevelBehaviour
    {
        private IBehaviour _child;

        public IEnumerable<IBehaviour> Childs {
            get {
                if(_child is null)
                    yield return ActionBehaviour.FAILURE;
                else
                    yield return _child;
            }
        }

        public DecoratorBehaviour(IBehaviour child)
            => AddChild(child);

        public bool AddChild(IBehaviour child)
        {
            if(child is null)
                return false;

            _child = child;

            return true;
        }
    }
}
