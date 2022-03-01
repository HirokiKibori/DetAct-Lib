using System.Linq;

using DetAct.Behaviour.Base;
using DetAct.Behaviour.Concrete;

using Xunit;
using Xunit.Abstractions;

namespace DetAct.Test.Behaviour.Concrete
{
    public class ConditionalTest : BasicTest
    {
        public ConditionalTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Childs_ShouldContain_ConditionAndChilds()
        {
            var testee = new Conditional(ConditionBehaviour.SUCCESS, ActionBehaviour.SUCCESS);

            Assert.NotEmpty(collection: testee.Childs);
            Assert.Equal(expected: 2, testee.Childs.Count());

            foreach(var child in testee.Childs) {
                output.WriteLine($"{child.GetType().Name}: '{child.Name}' -> {child.Status}");
            }
        }
    }
}
