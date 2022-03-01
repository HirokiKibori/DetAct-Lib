using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DetAct.Interpreter
{
    public class SemanticalAnalyzer
    {
        protected delegate List<InterpreterMessage> AnalyzeStatement(Statement statement, ref ImmutableList<Statement> statetemts);

        protected static readonly Dictionary<Type, Func<Statement, ImmutableList<Statement>, FunctionList, List<InterpreterMessage>>> StatementAnalyzers = new()
        {
            {
                typeof(BlackBoardDefinition),
                (statement, statements, functionList) => new BlackBoardDefinitionAnalyzer().Analyze(statement as BlackBoardDefinition, statements, functionList)
            },
            {
                typeof(Configuration),
                (statement, statements, functionList) => new ConfigurationAnalyzer().Analyze(statement as Configuration, statements, functionList)
            },
            {
                typeof(FunctionList),
                (statement, statements, functionList) => new FunctionListAnalyzer().Analyze(statement as FunctionList, statements, functionList)
            },
            {
                typeof(RootBehaviourStatement),
                (statement, statements, functionList) => new HighLevelBehaviourAnalyzer().Analyze(statement as HighLevelBehaviourStatement, statements, functionList)
            },
            {
                typeof(HighLevelBehaviourStatement),
                (statement, statements, functionList) => new HighLevelBehaviourAnalyzer().Analyze(statement as HighLevelBehaviourStatement, statements, functionList)
            },
            {
                typeof(LowLevelBehaviourStatement),
                (statement, statements, functionList) => new LowLevelBehaviourAnalyzer().Analyze(statement as LowLevelBehaviourStatement, statements, functionList)
            },
            {
                typeof(IndefinedStatement),
                (statement, statements, functionList) => new IndefinedStatementAnalyzer().Analyze(statement as IndefinedStatement, statements, functionList)
            }
        };

        protected SemanticalAnalyzer() { }

        public static List<InterpreterMessage> AnalyzeStatements(ImmutableList<Statement> statetemts)
        {
            List<InterpreterMessage> result = new();
            var functionList = statetemts.Find(statement => statement is FunctionList) as FunctionList;

            result.AddRange(StructureAnalyzer.Analyze(statetemts, functionList));

            foreach(Statement statement in statetemts) {
                var type = statement.GetType();

                if(StatementAnalyzers.TryGetValue(type, out var action))
                    result.AddRange(action(statement, statetemts, functionList));
            }

            return result;
        }
    }

    public class StructureAnalyzer
    {
        public static List<InterpreterMessage> Analyze(ImmutableList<Statement> statetemts, FunctionList functionList)
        {
            List<InterpreterMessage> result = new();

            // check for one root
            var roots = statetemts.FindAll(statement => statement is RootBehaviourStatement).Cast<RootBehaviourStatement>().ToList();
            if(roots.Count < 1)
                result.Add(
                    new ErrorMessage($"There must be exactly one ROOT-node. -> 'ROOT := ... ;'"));

            if(roots.Count > 1)
                result.Add(
                    new ErrorMessage($"There must be exactly one ROOT-node. ROOT-nodes detected: {roots.Count}"));

            // check for identifier-uniqueness (maybe a node searches for its own duplicates in the future)
            foreach(var statementToCheck in statetemts) {
                if(statementToCheck is not IndefinedStatement && statementToCheck is not RootBehaviourStatement &&
                    statetemts.Find(statement => statementToCheck.Name == statement.Name && !ReferenceEquals(statementToCheck, statement)) is not null)
                    result.Add(new ErrorMessage($"Duplicated identifier '{statementToCheck.Name}'."));
            }

            // FunctionList is a reserved identifier
            var countOfFunctionListName = statetemts.FindAll(statement => statement.Name == "FunctionList").Count;

            if(functionList is null)
                result.Add(new WarningMessage($"There must be exactly one 'FunctionList'. -> 'FunctionList := [ Func_Name(type param_name),  ...];' Supported types are 'board' and 'string'."));

            if(functionList is null && countOfFunctionListName > 0 ||
                functionList is not null && countOfFunctionListName > 1)
                result.Add(new ErrorMessage($"The identifier 'FunctionList' can only used for a FunctionList."));

            return result;
        }
    }

    public abstract class StatementAnalyzer<T> where T : Statement
    {
        public abstract List<InterpreterMessage> Analyze(T statement, ImmutableList<Statement> statetemts, FunctionList functionList);

        protected static bool HasParent(string statementName, IEnumerable<HighLevelBehaviourStatement> statetemts)
        {
            if(statementName == "ROOT")
                return true;

            foreach(var statement in statetemts) {
                if(statement.Childs.Find(child => child == statementName) is not null)
                    return true;
            }

            return false;
        }
    }

    public class BlackBoardDefinitionAnalyzer : StatementAnalyzer<BlackBoardDefinition>
    {
        public override List<InterpreterMessage> Analyze(BlackBoardDefinition statement, ImmutableList<Statement> statetemts, FunctionList functionList)
        {
            List<InterpreterMessage> result = new();

            var keys = statement.Items.Keys.ToList();

            foreach(var keyToCheck in keys) {
                // check for undefined keys
                if(string.IsNullOrWhiteSpace(keyToCheck))
                    result.Add(new ErrorMessage($"BlackBoard '{statement.Name}' contains an empty key."));

                // check for duplicated keys
                if(keys.FindAll(key => keyToCheck == key).Count > 1)
                    result.Add(new ErrorMessage($"BlackBoard '{statement.Name}' contains the duplicated key '{keyToCheck}'."));
            }

            return result;
        }
    }

    public class ConfigurationAnalyzer : StatementAnalyzer<Configuration>
    {
        public override List<InterpreterMessage> Analyze(Configuration statement, ImmutableList<Statement> statetemts, FunctionList functionList)
        {
            List<InterpreterMessage> result = new();

            if(string.IsNullOrWhiteSpace(statement.Value))
                result.Add(new ErrorMessage($"There is no value for {statement.Name}:Configuration."));

            return result;
        }
    }

    public class FunctionListAnalyzer : StatementAnalyzer<FunctionList>
    {
        public override List<InterpreterMessage> Analyze(FunctionList statement, ImmutableList<Statement> statetemts, FunctionList functionList)
        {
            List<InterpreterMessage> result = new();

            var names = statement.Functions.Select(function => function.Name).ToList();

            foreach(var nameToCheck in names) {
                if(string.IsNullOrWhiteSpace(nameToCheck))
                    result.Add(new ErrorMessage($"FunctionList contains an empty function-name."));

                // check for duplicated keys
                if(names.FindAll(name => nameToCheck == name).Count > 1)
                    result.Add(new ErrorMessage($"FunctionList contains the duplicated function-name '{statement.Name}'."));
            }

            return result;
        }
    }

    public class HighLevelBehaviourAnalyzer : StatementAnalyzer<HighLevelBehaviourStatement>
    {
        public override List<InterpreterMessage> Analyze(HighLevelBehaviourStatement statement, ImmutableList<Statement> statetemts, FunctionList functionList)
        {
            List<InterpreterMessage> result = new();

            if(statement.Childs.Count < 1)
                result.Add(new ErrorMessage($"'{statement.Name}:{statement.Type}' needs in minimum one child."));

            foreach(var child in statement.Childs) {
                if(statetemts.Find(statementsToCheck => statementsToCheck.Name == child) is null)
                    result.Add(new ErrorMessage($"'{statement.Name}:{statement.Type}' has a not defined child '{child}'."));
            }

            var behaviours = statetemts.FindAll(behaviourStatement => behaviourStatement is HighLevelBehaviourStatement).Cast<HighLevelBehaviourStatement>();
            if(!HasParent(statement.Name, behaviours))
                result.Add(new WarningMessage($"'{statement.Name}:{statement.Type}' has no parent."));


            return result;
        }
    }

    public class LowLevelBehaviourAnalyzer : StatementAnalyzer<LowLevelBehaviourStatement>
    {
        public override List<InterpreterMessage> Analyze(LowLevelBehaviourStatement statement, ImmutableList<Statement> statetemts, FunctionList functionList)
        {
            List<InterpreterMessage> result = new();

            var behaviours = statetemts.FindAll(behaviourStatement => behaviourStatement is HighLevelBehaviourStatement).Cast<HighLevelBehaviourStatement>();
            if(!HasParent(statement.Name, behaviours))
                result.Add(new WarningMessage($"'{statement.Name}:{statement.Type}' has no parent."));

            if(statement.Functions.IsEmpty)
                return result;

            foreach(var functionCall in statement.Functions) {
                Function? definedFunction = functionList?.Functions.ToList().FirstOrDefault(function => function.Name == functionCall.Name);

                if(definedFunction is null || (definedFunction?.Name is null && definedFunction?.Parameters is null)) {
                    result.Add(new ErrorMessage($"'{statement.Name}:{statement.Type}' uses a not defined function '{functionCall.Name}'."));

                    continue;
                }

                // check for defined types
                var parameters = definedFunction?.Parameters.ToArray();
                var values = functionCall.Values.ToArray();

                if(parameters is null)
                    continue;

                if(parameters.Length != values.Length) {
                    result.Add(new ErrorMessage($"'{statement.Name}:{statement.Type}' uses a not defined function '{functionCall.Name}' with {values.Length} parameters."));

                    continue;
                }

                for(int i = 0; i < parameters.Length; i++) {
                    if(parameters[i].Type == ParameterType.BOARD) {
                        if(values[i] is not BoardValue) {
                            result.Add(new ErrorMessage($"'{statement.Name}:{statement.Type}' parameter-type [{i}] in finction '{functionCall.Name}' should be a 'board'-type."));

                            continue;
                        }

                        var value = (BoardValue)values[i];
                        var blackBoards = statetemts.FindAll(blackBoard => blackBoard is BlackBoardDefinition).Cast<BlackBoardDefinition>().ToList();

                        CheckForMissingBlackboards(value, $"'{statement.Name}:{statement.Type}' parameter-type [{i}] in finction '{functionCall.Name}'", ref blackBoards, ref result);

                        continue;
                    }

                    if(parameters[i].Type == ParameterType.STRING) {
                        if(values[i] is not StringValue)
                            result.Add(new ErrorMessage($"'{statement.Name}:{statement.Type}' parameter-type [{i}] in finction '{functionCall.Name}' should be a 'string'-type."));

                        continue;
                    }

                    result.Add(new ErrorMessage($"'{statement.Name}:{statement.Type}' uses an undefined parameter-type [{i}] in function '{functionCall.Name}'."));
                }
            }

            return result;
        }

        private static void CheckForMissingBlackboards(BoardValue value, string errorMessagePrefix,
            ref List<BlackBoardDefinition> blackBoards, ref List<InterpreterMessage> result)
        {
            if(blackBoards.Find(blackBoard => blackBoard.Name == value.BoadName) is null)
                result.Add(new ErrorMessage($"{errorMessagePrefix} uses a indefined BlackBoard with name '{value.BoadName}'. -> '{value.BoadName}:BlackBoard := [(x, y), ...];' (with tuples of string)."));

            if(value.FieldDescriptor is BoardValue boardValue)
                CheckForMissingBlackboards(boardValue, errorMessagePrefix, ref blackBoards, ref result);
        }
    }

    public class IndefinedStatementAnalyzer : StatementAnalyzer<IndefinedStatement>
    {
        public override List<InterpreterMessage> Analyze(IndefinedStatement statement, ImmutableList<Statement> statetemts, FunctionList functionList)
        {
            List<InterpreterMessage> result = new();

            result.Add(new WarningMessage($"Indefined statement in line {statement.StartPosition.Line}: '{statement.Content}'"));
            // probably a parse-test of a single statement to check if there is a failor without filtering in IndefinedStatement.

            return result;
        }
    }
}
