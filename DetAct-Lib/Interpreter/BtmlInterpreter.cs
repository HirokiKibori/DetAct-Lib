using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using DetAct.Behaviour;

using Pidgin;

namespace DetAct.Interpreter
{
    public class BtmlInterpreter
    {
        protected BtmlInterpreter() { }

        public static InterpreterResult GenrateBehaviourTree(string btmlFileContent)
            => GenrateBehaviourTree(btmlFileContent, buildTreeDirector: new BtmlDirector(treeBuilder: new BehaviourTreeBuilder()));

        public static InterpreterResult GenrateBehaviourTree(string btmlFileContent, BtmlDirector buildTreeDirector)
        {
            List<InterpreterMessage> messages = new();

            try {
                var statetemts = ParseBtmlFileContent(btmlFileContent);

                //TODO: (later) add possibillity to add an own analyzer or remove an existing one to SemanticalAnalyzer (i.e. for IndefinedStatement)
                var semanticalFailures = SemanticalAnalyzer.AnalyzeStatements(statetemts);
                messages.AddRange(semanticalFailures);

                if(semanticalFailures.Find(failure => failure is ErrorMessage) is null) {
                    buildTreeDirector.ChangeStatements(statetemts);
                    var behaviourTree = buildTreeDirector.Construct().GetBehaviourTree()
                        ?? BehaviourTree.Default;

                    messages.AddRange(buildTreeDirector.Messages);

                    if(buildTreeDirector.Messages.Find(failure => failure is ErrorMessage) is null)
                        return new InterpreterResult(behaviourTree != BehaviourTree.Default, behaviourTree, messages);
                }
            }
            catch(Exception ex) {
                messages.Add(new ErrorMessage(ex.Message));
            }

            return new InterpreterResult(false, BehaviourTree.Default, messages);
        }

        protected static string ParseIndefinedStatement(IndefinedStatement statement)
        {
            try {
                var result = Parser.ParseStatementWithoutndefinedRule(statement.Content);

                return $"Statement indefined and will be ignored. {result}";
            }
            catch(ParseException ex) {
                return ex.Message;
            }
        }

        protected static ImmutableList<Statement> ParseBtmlFileContent(string btmlFileContent)
            => Parser.ParseBTML(btmlFileContent);
    }
}
