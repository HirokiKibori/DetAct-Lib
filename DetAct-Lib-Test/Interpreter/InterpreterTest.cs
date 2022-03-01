using System;

using DetAct.Behaviour;
using DetAct.Interpreter;
using DetAct.Test.Util;

using Xunit;
using Xunit.Abstractions;

namespace DetAct.Test.Interpreter
{
    public class InterpreterTest : BasicTest, IClassFixture<TestFile>
    {
        private TestFile testFile;

        public InterpreterTest(ITestOutputHelper output, TestFile testFile) : base(output)
            => this.testFile = testFile;

        [Fact]
        public void GenrateBehaviourTree_BtmlFileContents_NullEmptyOrWhitespace()
        {
            var testee = BtmlInterpreter.GenrateBehaviourTree(null);
            Assert.False(testee.IsValid);
            Assert.Equal(expected: BehaviourTree.Default, actual: testee.Tree);
            Assert.NotEmpty(testee.Messages);

            output.WriteLine("GenrateBehaviourTree(null)");
            foreach(var message in testee.Messages) {
                output.WriteLine($"{message.MessageType}: '{message.Message}'");
            }
            output.WriteLine(Environment.NewLine);

            testee = BtmlInterpreter.GenrateBehaviourTree("");
            Assert.False(testee.IsValid);
            Assert.Equal(expected: BehaviourTree.Default, actual: testee.Tree);
            Assert.NotEmpty(testee.Messages);

            output.WriteLine("GenrateBehaviourTree('')");
            foreach(var message in testee.Messages) {
                output.WriteLine($"{message.MessageType}: '{message.Message}'");
            }
            output.WriteLine(Environment.NewLine);

            testee = BtmlInterpreter.GenrateBehaviourTree(" ");
            Assert.False(testee.IsValid);
            Assert.Equal(expected: BehaviourTree.Default, actual: testee.Tree);
            Assert.Single(testee.Messages);
            Assert.Equal(expected: InterpreterMessageType.ERROR, actual: testee.Messages[0].MessageType);

            output.WriteLine("GenrateBehaviourTree(' ')");
            foreach(var message in testee.Messages) {
                output.WriteLine($"{message.MessageType}: '{message.Message}'");
            }
            output.WriteLine(Environment.NewLine);
        }

        [Fact]
        public void GenrateBehaviourTree_BtmlFileContents_ValidTree()
        {
            var testee = BtmlInterpreter.GenrateBehaviourTree(testFile.Content);
        }
    }
}
