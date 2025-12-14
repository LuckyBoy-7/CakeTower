using System.Collections;
using System.Collections.Generic;

namespace Lucky.Framework.CustomTick
{
    public class TickCoroutine : TickComponent
    {
        private Stack<IEnumerator> enumerators = new();
        private float waitTimer = -1;
        public bool Finished { get; private set; }
        public bool RemoveOnComplete = true;

        public TickCoroutine()
        {
        }

        public TickCoroutine(IEnumerator enumerator)
        {
            enumerators.Push(enumerator);
        }

        public override void Tick()
        {
            base.Tick();
            if (waitTimer > 0f)
            {
                waitTimer -= Engine.RawFixedDeltaTime;
                return;
            }

            if (enumerators.Count > 0)
            {
                IEnumerator topEnumerator = enumerators.Peek();
                if (topEnumerator.MoveNext() && !Finished)
                {
                    ParseCurrentValue(topEnumerator.Current);
                }
                else if (!Finished)  // 因为我们有可能在 Coroutine 里改变状态, 比如 state machine 在 coroutine 里切状态可能会导致栈为空
                {
                    enumerators.Pop();
                    if (enumerators.Count == 0)
                    {
                        Finished = true;
                        if (RemoveOnComplete)
                        {
                            RemoveSelf();
                        }
                    }
                }
            }
        }

        private void ParseCurrentValue(object current)
        {
            if (current is int i)
            {
                waitTimer = i;
            }
            else if (current is float f)
            {
                waitTimer = f;
            }
            else if (current is IEnumerator enumerator)
            {
                enumerators.Push(enumerator);
            }
        }

        public void Replace(IEnumerator enumerator)
        {
            Active = true;
            Finished = false;
            waitTimer = 0f;
            enumerators.Clear();
            enumerators.Push(enumerator);
        }

        public void Cancel()
        {
            Active = false;
            Finished = true;
            waitTimer = 0f;
            enumerators.Clear();
        }
    }
}