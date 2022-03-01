using System;
using System.Collections.Immutable;

using DetAct.Behaviour;
using DetAct.Interpreter;

using Xunit;
using Xunit.Abstractions;

namespace DetAct.Test.Behaviour
{
    public class BTMLDirectorTest : BasicTest
    {
        public BTMLDirectorTest(ITestOutputHelper output)
            : base(output) { }

        [Fact]
        public void Constructor_WithParameters_Null()
        {
            Assert.Throws<ArgumentNullException>(testCode: () => new BtmlDirector(null, null));
            Assert.Throws<ArgumentNullException>(testCode: () => new BtmlDirector(null, Array.Empty<Statement>().ToImmutableList()));

            _ = new BtmlDirector(new BehaviourTreeBuilder(), null);
        }

        [Fact]
        public void Constructor_WithParameters_NotNull()
        {
            var testee = new BtmlDirector(new BehaviourTreeBuilder(), Array.Empty<Statement>().ToImmutableList());

            Assert.NotNull(testee.TreeBuilder);
            Assert.IsType<BehaviourTreeBuilder>(testee.TreeBuilder);

            Assert.NotNull(testee.Statements);
            Assert.Empty(testee.Statements);

            Assert.NotEmpty(testee.RegisteredTypes);
        }
    }
}
