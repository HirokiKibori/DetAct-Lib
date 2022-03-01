using DetAct.Behaviour;
using DetAct.Test.Util.Behaviours;

using Xunit;

namespace DetAct.Test.Behaviour
{
    public class BehaviourTest
    {
        [Fact]
        public void Status_ShouldBeINDEFINITE_AfterInitialize()
        {
            TestBehaviour testee = new();

            Assert.Equal(expected: BehaviourStatus.INDEFINITE, actual: testee.Status);
        }

        [Fact]
        public void ResetStatus_ShouldSetStatusToINDEFINITE()
        {
            TestBehaviour testee = new();
            testee.ResetStatus();

            Assert.Equal(expected: BehaviourStatus.INDEFINITE, actual: testee.Status);
        }

        [Fact]
        public void Tick_IInitializable_SetToInitialized_IfNotRUNNING()
        {
            TestBehaviour testee = new();
            Assert.Equal(expected: BehaviourStatus.INDEFINITE, actual: testee.Status);

            Assert.Equal(expected: BehaviourStatus.RUNNING, actual: testee.Tick());

            Assert.True(condition: testee.IsInitialized);
            Assert.Equal(expected: BehaviourStatus.RUNNING, actual: testee.Status);

            Assert.False(condition: testee.IsInterrupted);
            Assert.False(condition: testee.IsTerminated);
        }

        [Fact]
        public void Tick_IInitializable_SetNotToInitialized_IfRUNNING()
        {
            TestBehaviour testee = new();
            testee.Status = BehaviourStatus.RUNNING;
            Assert.False(condition: testee.IsInitialized);

            Assert.Equal(expected: BehaviourStatus.RUNNING, actual: testee.Tick());

            Assert.False(condition: testee.IsInitialized);
            Assert.Equal(expected: BehaviourStatus.RUNNING, actual: testee.Status);

            Assert.False(condition: testee.IsInterrupted);
            Assert.False(condition: testee.IsTerminated);
        }
    }
}