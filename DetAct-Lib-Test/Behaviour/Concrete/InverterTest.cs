using DetAct.Behaviour;
using DetAct.Behaviour.Base;
using DetAct.Behaviour.Concrete;

using Xunit;

namespace DetAct.Test.Behaviour.Concrete
{
    public class InverterTest
    {
        [Fact]
        public void OnUpdate_ShouldInvert_SuccessToFailure()
        {
            var testee = new Inverter(ActionBehaviour.SUCCESS);

            Assert.Equal(expected: BehaviourStatus.FAILURE, actual: testee.Tick());
            Assert.NotEqual(expected: BehaviourStatus.SUCCESS, actual: testee.Tick());
        }

        [Fact]
        public void OnUpdate_ShouldInvert_FailureToSuccess()
        {
            var testee = new Inverter(ActionBehaviour.FAILURE);

            Assert.Equal(expected: BehaviourStatus.SUCCESS, testee.Tick());
            Assert.NotEqual(expected: BehaviourStatus.FAILURE, actual: testee.Tick());
        }

        [Fact]
        public void OnUpdate_ShouldNotInvert_Indefinite()
        {
            var testee = new Inverter(ActionBehaviour.INDEFINITE);

            Assert.Equal(expected: BehaviourStatus.INDEFINITE, actual: testee.Tick());
        }

        [Fact]
        public void OnUpdate_ShouldNotInvert_Interrupted()
        {
            var testee = new Inverter(ActionBehaviour.INTERRUPTED);

            Assert.Equal(expected: BehaviourStatus.INTERRUPTED, actual: testee.Tick());
        }

        [Fact]
        public void OnUpdate_ShouldNotInvert_Running()
        {
            var testee = new Inverter(ActionBehaviour.RUNNING);

            Assert.Equal(expected: BehaviourStatus.RUNNING, actual: testee.Tick());
        }
    }
}
