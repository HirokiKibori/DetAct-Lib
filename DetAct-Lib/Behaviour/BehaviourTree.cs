using System;
using System.Collections.Concurrent;

using DetAct.Behaviour.Base;
using DetAct.Behaviour.Concrete;

namespace DetAct.Behaviour
{
    public class BehaviourTree
    {
        public ConcurrentDictionary<string, Blackboard<string>> Boards { get; protected set; } = new();

        public static readonly BehaviourTree Default = new();

        public ConcurrentDictionary<string, string> Configurations { get; } = new();

        public string Name { get; set; } = "Default";

        public Root Root { get; private set; }

        public BehaviourStatus Status { get => Root.Status; }

        private BehaviourTree()
            => Root = new(ActionBehaviour.FAILURE);

        public BehaviourTree(Root root)
            => ChangeRoot(root ?? throw new ArgumentNullException(paramName: nameof(root)));

        public void ChangeRoot(Root root)
            => Root = root ?? new(ActionBehaviour.FAILURE);

        public bool AddBoard(string blackboardName, Blackboard<string> blackBoard)
            => Boards.TryAdd(key: blackboardName, value: blackBoard);

        public bool GetBoard(string blackboardName, out Blackboard<string> blackBoard)
            => Boards.TryGetValue(key: blackboardName, value: out blackBoard);

        public bool AddConfiguration(string name, string value) => Configurations.TryAdd(key: name, value);

        public string GetValueFromBoard(string blackboardName, string key)
        {
            if(GetBoard(blackboardName, out Blackboard<string> blackBoard)) {
                if(blackBoard.TryGetValue(key, out string value))
                    return value;
            }

            return "";
        }

        public BehaviourStatus Tick()
            => Root.Tick();

        public void Interrupt()
            => Behaviour.InterruptBehaviour(Root);
    }
}
