using DetAct.Behaviour;
using DetAct.Behaviour.Base;

using System;
using System.Linq;

using Xunit;

using ActionBehaviour = DetAct.Behaviour.Base.ActionBehaviour;

namespace DetAct.Test.Behaviour.Base
{
    public class DecoratorTest
    {
        private class TestDecorator : DecoratorBehaviour
        {
            public TestDecorator(IBehaviour child) : base(child) { }

            protected override BehaviourStatus OnUpdate() => BehaviourStatus.SUCCESS;
        }

        [Fact]
        public void Childs_ShouldHaveOnlyOneEntity_WhichIsNotNULL()
        {
            var testee = new TestDecorator(child: ActionBehaviour.SUCCESS);

            Assert.Single(testee.Childs);
            Assert.NotNull(testee.Childs.Single());
        }

        [Fact]
        public void Constructor_IfChildIsNULL_ChildSholdBeFailureAction()
        {
            var testee = new TestDecorator(child: null);

            Assert.Single(testee.Childs);
            Assert.NotNull(testee.Childs.Single());
            Assert.Equal(expected: ActionBehaviour.FAILURE, actual: testee.Childs.First());
        }
    }
}
