using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using DetAct.Behaviour.Base;
using DetAct.Behaviour.Concrete;
using DetAct.Interpreter;

namespace DetAct.Behaviour
{
    public class BtmlDirector : IBuildTreeDirector
    {
        public static readonly Dictionary<string, Type> DefaultTypes = new()
        {
            { "Inverter", typeof(Inverter) },
            { "Root", typeof(Root) },
            { "Selector", typeof(Selector) },
            { "Sequence", typeof(Sequence) },
            { "Action", typeof(FunctionalAction) },
            { "Condition", typeof(FunctionalCondition) },

            { "Repeater", typeof(Repeater) },
            { "Conditional", typeof(Conditional) },
            { "ParallelBehaviour", typeof(ParallelBehaviour) }
        };

        private ConcurrentDictionary<string, Type> _typeCache = new();

        public List<InterpreterMessage> Messages { get; } = new();

        public IBehaviourTreeBuilder TreeBuilder { get; private set; }

        public ImmutableList<Statement> Statements { get; private set; }

        public ImmutableDictionary<string, Type> RegisteredTypes { get => _typeCache.ToImmutableDictionary(); }

        public BtmlDirector() : this(treeBuilder: new BehaviourTreeBuilder()) { }

        public BtmlDirector(IBehaviourTreeBuilder treeBuilder) : this(treeBuilder, null) { }

        public BtmlDirector(IBehaviourTreeBuilder treeBuilder, ImmutableList<Statement> statements)
        {
            ChangeTreeBuilder(treeBuilder);
            ChangeStatements(statements);

            RegisterTypes(DefaultTypes);
        }

        public void ChangeTreeBuilder(IBehaviourTreeBuilder treeBuilder)
            => TreeBuilder = treeBuilder ?? throw new ArgumentNullException(paramName: nameof(treeBuilder));

        public void ChangeStatements(ImmutableList<Statement> statements)
            => Statements = statements ?? ImmutableList<Statement>.Empty;

        public bool RegisterType(string typeName, Type type)
        {
            if(type is null || !type.IsClass || type.IsAbstract || type.IsInterface)
                return false;

            if(typeName == "Root" && !type.IsAssignableTo(typeof(Root)))
                throw new ArgumentException(message: "Only a Root_type is assingable as 'Root'.", paramName: nameof(typeName));

            if(string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException(message: "A registered Type needs a concrete name.", paramName: nameof(typeName));

            if(type.GetInterface(nameof(IHighLevelBehaviour)) is null &&
                type.GetInterface(nameof(ILowLevelBehaviour)) is null)
                throw new ArgumentException(message: $"Only types of IHighLevelBehaviour or ILowLevelBehaviour allowed. '{type.FullName}'",
                    paramName: nameof(type));

            _typeCache[typeName] = type;

            return true;
        }

        public bool RegisterTypes(Dictionary<string, Type> types)
        {
            if(types is not null) {
                foreach(var typeDefinition in types) {
                    if(!RegisterType(typeDefinition.Key, typeDefinition.Value))
                        return false;
                }

                return true;
            }

            return false;
        }

        public bool UnregisterType(string typeName)
        {
            if(typeName == "Root")
                throw new ArgumentException(message: "Can not unregister 'Root'.", paramName: nameof(typeName));

            if(string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException(message: "There must be a concrete name to unregister a registered type.", paramName: nameof(typeName));

            return _typeCache.Remove(typeName, out _);
        }

        public void ClearRegisteredTypes()
            => _typeCache.Clear();

        public virtual IBehaviourTreeBuilder Construct()
        {
            TreeBuilder.Reset();
            Messages.Clear();

            Statements
                .FindAll(statement => statement is BlackBoardDefinition)
                .ForEach(blackboard => TreeBuilder.AddBlackboard(blackboard.Name, (blackboard as BlackBoardDefinition)?.Items));

            Statements
                .FindAll(statement => statement is Configuration)
                .ForEach(configuration => TreeBuilder.AddConfiguration(configuration.Name, (configuration as Configuration).Value));

            var rootStatement = Statements.Find(statement => statement is RootBehaviourStatement) as RootBehaviourStatement;
            if(!RegisteredTypes.TryGetValue("Root", out Type type))
                throw new TypeAccessException(message: "There is no registered behaviour-type 'Root' for a ROOT-node.");

            var root = TreeBuilder.AddRoot(rootStatement.Name, type);
            TreeBuilder.AddChildToParent(AddChild(rootStatement.Childs.Single()), root);

            return TreeBuilder;
        }

        private IBehaviour AddChild(string childIdentifier)
        {
            var statement = Statements.Find(statement => statement.Name == childIdentifier);

            if(statement is HighLevelBehaviourStatement)
                return CreateHighLevelBehaviour(statement as HighLevelBehaviourStatement);

            if(statement is LowLevelBehaviourStatement)
                return CreateLowLevelBehaviour(statement as LowLevelBehaviourStatement);

            throw new InvalidOperationException(message: $"The given statement '{statement.Name}' can not transformed to a highlevel- or lowlvel-behaviour.");
        }

        private IHighLevelBehaviour CreateHighLevelBehaviour(HighLevelBehaviourStatement behaviourStatement)
        {
            if(!RegisteredTypes.TryGetValue(behaviourStatement.Type, out Type type))
                throw new TypeAccessException(message: $"There is no registered behaviour-type '{behaviourStatement.Type}' for behaviour '{behaviourStatement.Name}'.");

            var behaviour = TreeBuilder.AddHighLevelBehaviour(behaviourStatement.Name, type);

            behaviour.ProcessParameterList(behaviourStatement.Parameters.ToArray());

            if(behaviour is DecoratorBehaviour && behaviourStatement.Childs.Count != 1) {
                Messages.Add(new WarningMessage($"{behaviourStatement.Name}:{behaviourStatement.Type} is a decorator-node and has not exactly one child-node. Only the last child in the node-list will be set as concrete child."));
            }

            foreach(var child in behaviourStatement.Childs) {
                TreeBuilder.AddChildToParent(AddChild(child), behaviour);
            }

            return behaviour;
        }

        private ILowLevelBehaviour CreateLowLevelBehaviour(LowLevelBehaviourStatement behaviourStatement)
        {
            if(!RegisteredTypes.TryGetValue(behaviourStatement.Type, out Type type))
                throw new TypeAccessException(message: $"There is no registered behaviour-type '{behaviourStatement.Type}' for behaviour '{behaviourStatement.Name}'.");

            var behaviour = TreeBuilder.AddLowLevelBehaviour(behaviourStatement.Name, type);

            behaviour.ProcessParameterList(behaviourStatement.Parameters.ToArray());

            if(behaviour is IBtmlLowLevelBehaviour)
                (behaviour as IBtmlLowLevelBehaviour).Functions = behaviourStatement.Functions;

            return behaviour;
        }
    }
}
