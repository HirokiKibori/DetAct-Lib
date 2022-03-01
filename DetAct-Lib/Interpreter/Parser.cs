using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Pidgin;
using Pidgin.Comment;

using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace DetAct.Interpreter
{
    public class Parser
    {
        /// <summary>
        /// base-parsers for symbols (like the tutorial)
        /// </summary>
        #region skip-whitespace-symbols
        private static Parser<char, T> Symbol<T>(Parser<char, T> parser)
            => parser.Before(SkipWhitespaces);
        private static Parser<char, char> Symbol(char value)
           => Symbol(Char(value));
        private static Parser<char, string> Symbol(string value)
            => Symbol(String(value));
        #endregion

        #region block-symbols
        private static readonly Parser<char, string> _start_comment_symbol = Symbol("/*");
        private static readonly Parser<char, string> _end_comment_symbol = Symbol("*/");

        private static readonly Parser<char, char> _start_tuple_symbol = Symbol('(');
        private static readonly Parser<char, char> _end_tuple_symbol = Symbol(')');

        private static readonly Parser<char, char> _start_list_symbol = Symbol('[');
        private static readonly Parser<char, char> _end_list_symbol = Symbol(']');

        private static readonly Parser<char, char> _start_black_board_field_symbol = Symbol('[');
        private static readonly Parser<char, char> _end_black_board_field_symbol = Symbol(']');

        private static readonly Parser<char, char> _start_function_parameters_symbol = Symbol('(');
        private static readonly Parser<char, char> _end_function_parameters_symbol = Symbol(')');

        private static readonly Parser<char, char> _start_behaviour_parameters_symbol = Symbol('{');
        private static readonly Parser<char, char> _end_behaviour_parameters_symbol = Symbol('}');
        #endregion

        #region single-symbols
        private static readonly Parser<char, string> _definition_symbol = Symbol(":=");
        private static readonly Parser<char, char> _type_symbol = Symbol(':');
        private static readonly Parser<char, char> _node_split_symbol = Symbol('|');
        private static readonly Parser<char, char> _tuple_separator_symbol = Symbol(',');
        private static readonly Parser<char, char> _list_separator_symbol = Symbol(',');

        private static readonly Parser<char, char> _statement_end_symbol = Symbol(';');

        private static readonly Parser<char, char> _underscore_symbol = Char('_');

        private static readonly Parser<char, char> _parameter_string_symbol = Char('"');
        #endregion

        #region reserved-symbols
        private static readonly Parser<char, string> _root_keyword = Symbol("ROOT");
        private static readonly Parser<char, string> _function_list_keyword = Symbol("FunctionList");

        private static readonly Parser<char, string> _blackboard_type = Symbol("BlackBoard");
        private static readonly Parser<char, string> _config_type = Symbol("Config");

        private static readonly Parser<char, string> _parameter_type_string = Symbol("string");
        private static readonly Parser<char, string> _parameter_type_board = Symbol("board");

        private static readonly Parser<char, char> _any_unreserved_symbol =
            AnyCharExcept(new char[] {
                    '(', ')', '[', ']', '{', '}',
                    ':', '|', '=',
                    ',', ';',
                    '"', '\'',
                    '*', '+',
                    '\\'
                });

        //private static readonly Parser<char, string> _boolean_true = Symbol("true");
        //private static readonly Parser<char, string> _boolean_false = Symbol("false");
        //private static readonly Parser<char, string> _null = Symbol("null");
        #endregion

        #region helper
        private static readonly Parser<char, string> Identifier
             = from firstLetter in Letter.AtLeastOnceString()
               from rest in Try(LetterOrDigit).Or(_underscore_symbol).ManyString()
               select firstLetter + rest;

        private static Parser<char, string> Type(Parser<char, string> typeName)
             => from _ in _type_symbol
                from type in typeName.Before(SkipWhitespaces)
                select type;

        private static Parser<char, string> AnyUntil<T>(Parser<char, T> terminator)
             => from cs in Any.Until(terminator)
                select string.Concat(cs);
        #endregion

        #region list
        private static Parser<char, ImmutableList<T>> List<T>(Parser<char, T> parser)
             => from _ in _start_list_symbol
                from items in parser.Separated(_list_separator_symbol).Before(_end_list_symbol)
                select items.ToImmutableList();
        #endregion

        #region pair
        private static Parser<char, KeyValuePair<string, string>> Tuple
             => from _ in _start_tuple_symbol
                from pair in TupleItems.Before(_end_tuple_symbol)
                select pair;

        private static Parser<char, KeyValuePair<string, string>> TupleItems
             => from firstItem in _any_unreserved_symbol.AtLeastOnceString().Before(_tuple_separator_symbol)
                from secondItem in _any_unreserved_symbol.AtLeastOnceString()
                select new KeyValuePair<string, string>(firstItem, secondItem);
        #endregion

        #region values
        private static Parser<char, IValue> ParameterValue
             => from parameter in OneOf(StringValue, /*NumValue, RealValue, BoolValue, NullValue,*/ BlackBoardValue)
                select parameter;

        private static Parser<char, IValue> StringValue
             => from _ in _parameter_string_symbol
                from value in AnyUntil(_parameter_string_symbol)
                select new StringValue(value) as IValue;

        private static Parser<char, IValue> BlackBoardValue
             => Rec(() =>
                    from boardName in Identifier
                    from _ in _start_black_board_field_symbol
                    from fieldDescriptor in OneOf(Try(BlackBoardValue), FieldDescriptor).Before(_end_black_board_field_symbol)
                    select new BoardValue(boardName, fieldDescriptor) as IValue
                );
        private static Parser<char, IValue> FieldDescriptor
             => from value in Identifier
                select new FieldDescriptor(value) as IValue;

        /*
        private static Parser<char, IValue> NumValue
             => from value in LongNum
                select value as IValue;

        private static Parser<char, IValue> RealValue
             => from value in Real
                select value as IValue;

        private static Parser<char, IValue> BoolValue
             => from value in OneOf(_boolean_true, _boolean_false)
                select value as IValue;

        private static Parser<char, IValue> NullValue
             => from value in _null
                select value as IValue;
        */
        #endregion

        #region functions
        private static readonly Parser<char, Function> _function_rule
             = from name in Identifier.Before(_start_function_parameters_symbol)
               from parameterDefinitions in ParameterDefinition.Separated(_list_separator_symbol).Before(_end_function_parameters_symbol)
               select new Function(name, parameterDefinitions);

        private static Parser<char, ParameterDefiniton> ParameterDefinition
             => from type in OneOf(_parameter_type_string, _parameter_type_board)
                from name in Identifier.Before(SkipWhitespaces)
                select new ParameterDefiniton(type, name);

        private static Parser<char, FunctionCall> FunctionCall
             => from name in Identifier.Before(_start_function_parameters_symbol)
                from values in ParameterValue.Separated(_list_separator_symbol).Before(_end_function_parameters_symbol)
                select new FunctionCall(name, values);
        #endregion

        #region statements
        private static Parser<char, IEnumerable<string>> BehaviourParameters
             => from _ in _start_behaviour_parameters_symbol
                from values in LetterOrDigit.ManyString().Separated(_list_separator_symbol).Before(_end_behaviour_parameters_symbol)
                select values;

        private static readonly Parser<char, Statement> _root_rule
             = from name in _root_keyword
               from _ in _definition_symbol
               from child in Identifier.Before(SkipWhitespaces)
               select new RootBehaviourStatement(name, child) as Statement;

        private static readonly Parser<char, Statement> _config_rule
             = from name in Identifier
               from _ in Type(_config_type).Before(_definition_symbol)
               from value in _parameter_string_symbol.Then(AnyUntil(_parameter_string_symbol))
               select new Configuration(name, value) as Statement;

        private static readonly Parser<char, Statement> _black_board_rule
             = from name in Identifier
               from _ in Type(_blackboard_type).Before(_definition_symbol)
               from items in List(Tuple)
               select new BlackBoardDefinition(name, items) as Statement;

        private static readonly Parser<char, Statement> _function_list_rule
             = from _ in _function_list_keyword.Before(_definition_symbol)
               from functions in List(_function_rule)
               select new FunctionList("FunctionList", functions) as Statement;

        private static readonly Parser<char, Statement> _lowlevel_behaviour_with_definition_rule
             = from name in Identifier
               from type in Type(Identifier)
               from parameters in BehaviourParameters.Optional().Before(_definition_symbol)
               from functionCalls in Try(List(FunctionCall.Before(SkipWhitespaces))).Or(FunctionCall.Before(SkipWhitespaces).Map(funcCall => ImmutableList.Create(funcCall)))
               select new LowLevelBehaviourStatement(name, type, functionCalls, parameters.HasValue ? parameters.Value : Array.Empty<string>()) as Statement;

        private static readonly Parser<char, Statement> _lowlevel_behaviour_without_definition_rule
             = from name in Identifier
               from type in Type(Identifier)
               from parameters in BehaviourParameters.Optional()
               select new LowLevelBehaviourStatement(name, type, null, parameters.HasValue ? parameters.Value : Array.Empty<string>()) as Statement;

        private static readonly Parser<char, Statement> _highlevel_behaviour_rule
             = from name in Identifier
               from type in Type(Identifier)
               from parameters in BehaviourParameters.Optional().Before(_definition_symbol)
               from childs in Identifier.Before(SkipWhitespaces).SeparatedAtLeastOnce(_node_split_symbol)
               select new HighLevelBehaviourStatement(name, type, childs, parameters.HasValue ? parameters.Value : Array.Empty<string>()) as Statement;

        private static readonly Parser<char, Statement> _indefined_rule
             = Map((startPosition, text, endPosition)
                 => string.IsNullOrWhiteSpace(text)
                        ? Statement.Default
                        : new IndefinedStatement(startPosition, endPosition, content: $"{text};") as Statement,
                 CurrentPos,
                 AnyUntil(_statement_end_symbol),
                 CurrentPos);
        #endregion

        private static Parser<char, T> TryParseStatementRule<T>(Parser<char, T> parser)
             => Try(parser.Before(_statement_end_symbol));

        private static Parser<char, Statement> StatementParser(Parser<char, Statement> parser)
             => SkipWhitespaces
            .Then(
                 CommentParser.SkipBlockComment(_start_comment_symbol, Try(_end_comment_symbol))
                 .WithResult(Statement.Default)
                 .Or(parser)
             );

        private static readonly Parser<char, Statement> _statement
             = OneOf(
                 TryParseStatementRule(_root_rule),
                 TryParseStatementRule(_config_rule),

                 TryParseStatementRule(_black_board_rule),
                 TryParseStatementRule(_function_list_rule),

                 TryParseStatementRule(_lowlevel_behaviour_with_definition_rule),
                 TryParseStatementRule(_highlevel_behaviour_rule),
                 TryParseStatementRule(_lowlevel_behaviour_without_definition_rule)
               );

        private static readonly Parser<char, ImmutableList<Statement>> _btml
             = StatementParser(_statement.Or(_indefined_rule)).Many()
            .Select(statements => statements.ToImmutableList()
            .FindAll(statement => !statement.Equals(Statement.Default)
            ));

        #region interface
        public static Statement ParseStatement(string input) => StatementParser(_statement.Or(_indefined_rule)).ParseOrThrow(input);

        public static Statement ParseStatementWithoutndefinedRule(string input) => StatementParser(_statement).ParseOrThrow(input);

        public static ImmutableList<Statement> ParseBTML(string input) => _btml.ParseOrThrow(input);
        #endregion
    }
}
