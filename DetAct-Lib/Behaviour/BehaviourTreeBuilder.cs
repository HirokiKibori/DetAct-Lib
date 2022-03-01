using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using DetAct.Behaviour.Concrete;

namespace DetAct.Behaviour
{
    public class BehaviourTreeBuilder : IBehaviourTreeBuilder
    {
        protected BehaviourTree behaviourTree = null;

        public BehaviourTreeBuilder() => Reset();

        public void AddBlackboard(string name, Dictionary<string, string> initialValues)
            => behaviourTree.AddBoard(name, new Blackboard<string>(initialValues));

        public void AddChildToParent(IBehaviour child, IHighLevelBehaviour parent)
            => parent?.AddChild(child);

        public void AddConfiguration(string mane, string value) => behaviourTree.AddConfiguration(mane, value);

        public IHighLevelBehaviour AddHighLevelBehaviour(string name, Type type)
        {
            if(!type.GetInterfaces().Contains(typeof(IHighLevelBehaviour)))
                throw new InvalidCastException(message: $"{nameof(type)} '{type.Name}' from behaviour '{name}' can not converted to '{typeof(IHighLevelBehaviour).FullName}'.");

            var behaviour = Activator.CreateInstance(type, true) as IHighLevelBehaviour;
            behaviour.Name = name;

            return behaviour;
        }

        public ILowLevelBehaviour AddLowLevelBehaviour(string name, Type type)
        {
            if(!type.GetInterfaces().Contains(typeof(ILowLevelBehaviour)))
                throw new InvalidCastException(message: $"{nameof(type)} '{type.Name}' from behaviour '{name}' can not converted to '{typeof(ILowLevelBehaviour).FullName}'.");

            var behaviour = Activator.CreateInstance(type, true) as ILowLevelBehaviour;

            behaviour.Name = name;
            behaviour.ParentTree = behaviourTree;

            return behaviour;
        }

        public Root AddRoot(string name, Type type)
        {
            if(!type.IsAssignableTo(typeof(Root)))
                throw new InvalidCastException(message: $"{nameof(type)} from behaviour '{name}' can not converted to '{typeof(Root).FullName}'.");

            var root = Activator.CreateInstance(type, true) as Root;
            root.Name = name;
            behaviourTree.ChangeRoot(root);

            return root;
        }

        public BehaviourTree GetBehaviourTree()
            => behaviourTree;

        public void Reset()
            => behaviourTree = Activator.CreateInstance(typeof(BehaviourTree), true) as BehaviourTree;
    }
}