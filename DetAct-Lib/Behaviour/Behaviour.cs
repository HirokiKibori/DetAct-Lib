using System;
using System.Collections.Generic;

namespace DetAct.Behaviour
{
    public abstract class Behaviour : IBehaviour
    {
        private object lockTick = new();

        public string Name { get; set; } = "";

        public BehaviourStatus Status { get; protected set; } = BehaviourStatus.INDEFINITE;

        public void ResetStatus() => Status = BehaviourStatus.INDEFINITE;

        public static void InterruptBehaviour(IBehaviour Behaviour)
        {
            //TODO: Reworke! (add to language and dont interrupt a node by itselfe when calling to root while update)
            _ = Behaviour ?? throw new ArgumentNullException(paramName: nameof(Behaviour));

            if(Behaviour is IHighLevelBehaviour) {
                var childs = (Behaviour as IHighLevelBehaviour).Childs;

                foreach(var child in childs) {
                    InterruptBehaviour(Behaviour: child);
                }
            }

            if(Behaviour.Status is BehaviourStatus.RUNNING) {
                (Behaviour as Behaviour).Status = BehaviourStatus.INTERRUPTED;

                if(Behaviour is IInterruptible)
                    (Behaviour as IInterruptible).Interrupt();
            }

            Behaviour.ResetStatus();
        }

        public BehaviourStatus Tick()
        {
            var result = BehaviourStatus.INDEFINITE;

            lock(lockTick) {
                if(this is IInitializable && Status is not BehaviourStatus.RUNNING) {
                    (this as IInitializable).OnInitialize();
                }

                result = OnUpdate();
                Status = result;
                TaskAfterUpdate();

                if(this is IInitializable && Status is not BehaviourStatus.RUNNING) {
                    (this as IInitializable).OnTerminate(status: result);
                }
            }

            return Status;
        }

        //TODO: Think about old process
        /*
        public BehaviourStatus Tick()
        {
            var result = BehaviourStatus.INDEFINITE;

            lock(lockTick) {
                if(this is IInitializable && Status is not BehaviourStatus.RUNNING) {
                    (this as IInitializable).OnInitialize();
                }

                result = OnUpdate();
                Status = result;
                TaskAfterUpdate();

                if(this is IInitializable && Status is not BehaviourStatus.RUNNING) {
                    (this as IInitializable).OnTerminate(status: Status);
                }
            }

            return result;
        }
        */

        //TODO: implement as interface and concurrentdictionary and add it to language
        protected virtual void TaskAfterUpdate() { }

        public virtual List<string> GetBehaviourInfo() => new();

        public virtual void ProcessParameterList(string[] values) { }

        protected abstract BehaviourStatus OnUpdate();
    }
}
