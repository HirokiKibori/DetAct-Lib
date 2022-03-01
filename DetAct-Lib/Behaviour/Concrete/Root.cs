using System;
using System.Linq;

using DetAct.Behaviour.Base;

namespace DetAct.Behaviour.Concrete
{
    public class Root : DecoratorBehaviour
    {
        protected Root() : base(null) { }

        public Root(IBehaviour child) : base(child ?? throw new ArgumentNullException(paramName: nameof(child))) { }

        protected override BehaviourStatus OnUpdate() => Childs.Single().Tick();
    }
}
