using System;
using System.Collections;
using UnityEngine;

namespace Lucky.Framework.CustomTick
{
    [Serializable]
    public class TickStateMachine : TickComponent
    {
        private MonoBehaviour entity;
        [SerializeField] private int state;
        [SerializeField] private string stateName;
        private string[] names;
        private Action[] begins;
        private Action[] ends;
        private Func<int>[] updates;
        private Func<IEnumerator>[] coroutines;
        private TickCoroutine currentCoroutine;

        public bool Log;
        public int PreviousState { get; private set; }

        public delegate void _OnStateChanged(int previouseState, int currentState);

        public event _OnStateChanged OnStateChanged;

        public TickStateMachine(MonoBehaviour entity, int maxStates = 10)
        {
            this.entity = entity;
            PreviousState = (state = -1);
            names = new string[maxStates];
            begins = new Action[maxStates];
            ends = new Action[maxStates];
            updates = new Func<int>[maxStates];
            coroutines = new Func<IEnumerator>[maxStates];
            currentCoroutine = null;

            currentCoroutine = new TickCoroutine() { RemoveOnComplete = false };
        }

        public int State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    if (Log)
                    {
                        Debug.Log(string.Concat("Enter State ", names[value], " (leaving ", names[state], ")"));
                    }

                    PreviousState = state;
                    state = value;
                    stateName = names[state];
                    if (PreviousState != -1 && ends[PreviousState] != null)
                    {
                        if (Log)
                        {
                            Debug.Log("Calling End " + names[PreviousState]);
                        }

                        ends[PreviousState]();
                    }

                    if (begins[state] != null)
                    {
                        if (Log)
                        {
                            Debug.Log("Calling Begin " + names[state]);
                        }

                        begins[state]();
                    }


                    currentCoroutine.Cancel();
                    if (coroutines[state] != null)
                    {
                        if (Log)
                        {
                            Debug.Log("Starting Coroutine " + names[state]);
                        }

                        currentCoroutine.Replace(coroutines[state]());
                    }

                    OnStateChanged?.Invoke(PreviousState, value);
                }
            }
        }


        public void SetCallbacks(int state, string name, Action begin = null, Action end = null, Func<int> onUpdate = null, Func<IEnumerator> coroutine = null)
        {
            names[state] = name;
            begins[state] = begin;
            ends[state] = end;
            updates[state] = onUpdate;
            coroutines[state] = coroutine;
        }

        public override void Tick()
        {
            base.Tick();

            if (updates[state] != null)
            {
                State = updates[state]();
            }

            if (currentCoroutine.Active)
            {
                currentCoroutine.Tick();
            }
        }

        public static implicit operator int(TickStateMachine s)
        {
            return s.state;
        }

        public bool AnyEqual(params int[] states)
        {
            foreach (int state in states)
            {
                if (state == State)
                    return true;
            }

            return false;
        }
    }
}