using System;

using DetAct.Interpreter;

using Xunit;
using Xunit.Abstractions;

namespace DetAct.Test.Interpreter
{
    public class ParserTest : BasicTest
    {
        public ParserTest(ITestOutputHelper output)
            : base(output) { }

        [Fact]
        public void ParseStatement_NullEmptyWhitespace_ThrowsParseException()
        {
            Assert.Throws<Pidgin.ParseException>(testCode: () => Parser.ParseStatement(null));
            Assert.Throws<Pidgin.ParseException>(testCode: () => Parser.ParseStatement(string.Empty));
            Assert.Throws<Pidgin.ParseException>(testCode: () => Parser.ParseStatement(" "));
        }

        [Fact]
        public void ParseStatement_StatementTerminator_ReturnsDefaultStatement()
        {
            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement(";"));
            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement(" ;"));
            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement("  ;"));

            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement("\n;"));
            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement("\n\r;"));
            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement(Environment.NewLine + ";"));

            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement("\t;"));
        }

        [Fact]
        public void ParseStatement_StatementTerminator_IgnoreComment()
        {
            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement("/* testee */;"));
            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement("/* testee *//* testee */;"));
            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement("/* testee */ /* testee */;"));
            Assert.Equal(expected: Statement.Default, actual: Parser.ParseStatement(" /* testee */ /* testee */ ;"));

            Assert.Throws<Pidgin.ParseException>(testCode: () => Parser.ParseStatement("/*;"));

            var testee = "*/;";
            var result = Parser.ParseStatement(testee);
            Assert.IsType<IndefinedStatement>(result);

            var indefinedStatement = result as IndefinedStatement;
            Assert.Equal(expected: "*/;", actual: indefinedStatement.Content);
            Assert.Equal(expected: 1, actual: indefinedStatement.StartPosition.Line);
            Assert.Equal(expected: 1, actual: indefinedStatement.StartPosition.Col);
            Assert.Equal(expected: 1, actual: indefinedStatement.EndPosition.Line);
            Assert.Equal(expected: testee.Length + 1, actual: indefinedStatement.EndPosition.Col);
        }

        [Fact]
        public void ParseStatement_BlackBoard_Empty()
        {
            var result = Parser.ParseStatement("NameTest:BlackBoard := [];");
            Assert.IsType<BlackBoardDefinition>(result);

            var bb = result as BlackBoardDefinition;
            Assert.Equal(expected: "NameTest", actual: bb.Name);
            Assert.Empty(collection: bb.Items);
        }

        [Fact]
        public void ParseStatement_BlackBoard_Empty_With_UnderscoreInName()
        {
            var result = Parser.ParseStatement("Name_Test:BlackBoard := [];");
            Assert.IsType<BlackBoardDefinition>(result);

            var bb = result as BlackBoardDefinition;
            Assert.Equal(expected: "Name_Test", actual: bb.Name);
            Assert.Empty(collection: bb.Items);
        }

        [Fact]
        public void ParseStatement_BlackBoard_OneItem()
        {
            var result = Parser.ParseStatement("NameTest:BlackBoard := [(a,b)];");
            Assert.IsType<BlackBoardDefinition>(result);

            var bb = result as BlackBoardDefinition;
            Assert.Equal(expected: "NameTest", actual: bb.Name);
            Assert.Single(collection: bb.Items);

            Assert.Equal(expected: "b", actual: bb.Items["a"]);
        }

        [Fact]
        public void ParseStatement_BlackBoard_TwoItems()
        {
            var result = Parser.ParseStatement("NameTest:BlackBoard := [(a,b),(c,d)];");
            Assert.IsType<BlackBoardDefinition>(result);

            var bb = result as BlackBoardDefinition;
            Assert.Equal(expected: "NameTest", actual: bb.Name);
            Assert.Equal(expected: 2, actual: bb.Items.Count);

            Assert.Equal(expected: "b", actual: bb.Items["a"]);
            Assert.Equal(expected: "d", actual: bb.Items["c"]);
        }

        [Fact]
        public void ParseStatement_HighLevelBehaviour_Empty()
        {
            var result = Parser.ParseStatement("NameTest:Sequence := ;");
            Assert.IsType<IndefinedStatement>(result);
        }

        [Fact]
        public void ParseStatement_HighLevelBehaviour_OneChild()
        {
            var result = Parser.ParseStatement("NameTest:Sequence := ChildA;");
            Assert.IsType<HighLevelBehaviourStatement>(result);

            var bb = result as HighLevelBehaviourStatement;
            Assert.Equal(expected: "NameTest", actual: bb.Name);
            Assert.Equal(expected: "Sequence", actual: bb.Type);
            Assert.Single(collection: bb.Childs);

            Assert.Equal(expected: "ChildA", actual: bb.Childs[0]);
        }

        [Fact]
        public void ParseStatement_HighLevelBehaviour_TworChilds()
        {
            var result = Parser.ParseStatement("NameTest:Sequence := ChildA | ChildB;");
            Assert.IsType<HighLevelBehaviourStatement>(result);

            var bb = result as HighLevelBehaviourStatement;
            Assert.Equal(expected: "NameTest", actual: bb.Name);
            Assert.Equal(expected: "Sequence", actual: bb.Type);
            Assert.Equal(expected: 2, actual: bb.Childs.Count);

            Assert.Equal(expected: "ChildA", actual: bb.Childs[0]);
            Assert.Equal(expected: "ChildB", actual: bb.Childs[1]);
        }

        [Fact]
        public void ParseStatement_LowLevelBehaviour_WithoutFunctionDefinition()
        {
            var result = Parser.ParseStatement("NameTest:Action;");

            if(result is IndefinedStatement)
                output.WriteLine($"{(result as IndefinedStatement).Content}");

            Assert.IsType<LowLevelBehaviourStatement>(result);

            var testee = result as LowLevelBehaviourStatement;
            Assert.Equal(expected: "NameTest", actual: testee.Name);
            Assert.Equal(expected: "Action", actual: testee.Type);
            Assert.Empty(testee.Functions);
            Assert.Empty(collection: testee.Parameters);
        }
    }
}
