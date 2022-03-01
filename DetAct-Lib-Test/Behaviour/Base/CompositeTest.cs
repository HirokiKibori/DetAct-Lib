using DetAct.Behaviour;
using DetAct.Behaviour.Base;
using DetAct.Test.Util.Behaviours;

using System.Linq;

using Xunit;

namespace DetAct.Test.Behaviour.Base
{
    public class CompositeTest
    {
        private class TestComposite : CompositeBehaviour
        {
            public TestComposite(IBehaviour child) : base(child) { }

            protected override BehaviourStatus OnUpdate() => BehaviourStatus.SUCCESS;
        }

        [Fact]
        public void Childs_ShouldHaveThreeEntities_DuplicationsAreIgnored()
        {
            CompositeBehaviour testee = new TestComposite(child: ActionBehaviour.SUCCESS);

            Assert.False(testee.AddChild(child: ActionBehaviour.SUCCESS));
            Assert.True(testee.AddChild(child: ActionBehaviour.FAILURE));
            Assert.True(testee.AddChild(child: ActionBehaviour.INDEFINITE));

            Assert.Equal(expected: 3, actual: testee.Childs.Count());
        }
    }
}
