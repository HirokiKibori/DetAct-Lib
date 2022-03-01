using DetAct.Interpreter;
using DetAct.Test.Util;

using Xunit;
using Xunit.Abstractions;

namespace DetAct.Test.Interpreter
{
    public class ParseBTMLFileTest : BasicTest, IClassFixture<TestFile>
    {
        private TestFile testFile;

        public ParseBTMLFileTest(ITestOutputHelper output, TestFile testFile) : base(output)
            => this.testFile = testFile;

        [Fact]
        public void ParseBTML_ContainsNo_IndefinedStatement()
        {
            var result = Parser.ParseBTML(testFile.Content);

            foreach(var statement in result) {
                output.WriteLine($"{statement.GetType().Name}: {statement}");
            }
        }
    }
}
