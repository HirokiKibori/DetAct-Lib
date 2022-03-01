using System;
using System.Collections.Generic;

using DetAct.Behaviour.Concrete;

namespace DetAct.Behaviour
{
    public interface IBehaviourTreeBuilder
    {
        public void Reset();

        public void AddBlackboard(string name, Dictionary<string, string> initialValues);

        public void AddConfiguration(string name, string value);

        public Root AddRoot(string name, Type type);

        public IHighLevelBehaviour AddHighLevelBehaviour(string name, Type type);

        public ILowLevelBehaviour AddLowLevelBehaviour(string name, Type type);

        public void AddChildToParent(IBehaviour childName, IHighLevelBehaviour parentName);

        public BehaviourTree GetBehaviourTree();
    }
}
