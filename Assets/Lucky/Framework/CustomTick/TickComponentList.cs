using System;
using System.Collections;
using System.Collections.Generic;

namespace Lucky.Framework.CustomTick
{
    public class TickComponentList : IEnumerable<TickComponent>, IEnumerable
    {
        private List<TickComponent> components;
        private List<TickComponent> toAdd;
        private List<TickComponent> toRemove;
        private HashSet<TickComponent> current;
        private HashSet<TickComponent> adding;
        private HashSet<TickComponent> removing;

        private LockModes lockMode;

        public enum LockModes
        {
            Open,
            Locked,
            Error
        }

        public TickBehaviour parent { get; private set; }

        public TickComponentList(TickBehaviour parent)
        {
            this.parent = parent;
            components = new List<TickComponent>();
            toAdd = new List<TickComponent>();
            toRemove = new List<TickComponent>();
            current = new HashSet<TickComponent>();
            adding = new HashSet<TickComponent>();
            removing = new HashSet<TickComponent>();
        }

        public LockModes LockMode
        {
            get => lockMode;
            set
            {
                lockMode = value;
                if (toAdd.Count > 0)
                {
                    foreach (TickComponent component in toAdd)
                    {
                        if (!current.Contains(component))
                        {
                            // 使用 current 和 components 保证了快速查找的同时, 又能保持组件调用顺序
                            current.Add(component);
                            components.Add(component);
                            component.Added(parent);
                        }
                    }

                    adding.Clear();
                    toAdd.Clear();
                }

                if (toRemove.Count > 0)
                {
                    foreach (TickComponent component2 in toRemove)
                    {
                        if (current.Contains(component2))
                        {
                            current.Remove(component2);
                            components.Remove(component2);
                            component2.Removed(parent);
                        }
                    }

                    removing.Clear();
                    toRemove.Clear();
                }
            }
        }

        public void Add(TickComponent component)
        {
            switch (lockMode)
            {
                case LockModes.Open:
                    if (!current.Contains(component))
                    {
                        current.Add(component);
                        components.Add(component);
                        component.Added(parent);
                    }

                    break;
                case LockModes.Locked:
                    if (!current.Contains(component) && !adding.Contains(component))
                    {
                        adding.Add(component);
                        toAdd.Add(component);
                    }

                    break;
                case LockModes.Error:
                    throw new Exception("Cannot add or remove Entities at this time!");
                default:
                    return;
            }
        }

        public void Remove(TickComponent component)
        {
            switch (lockMode)
            {
                case LockModes.Open:
                    if (current.Contains(component))
                    {
                        current.Remove(component);
                        components.Remove(component);
                        component.Removed(parent);
                    }

                    break;
                case LockModes.Locked:
                    if (current.Contains(component) && !removing.Contains(component))
                    {
                        removing.Add(component);
                        toRemove.Add(component);
                    }

                    break;
                case LockModes.Error:
                    throw new Exception("Cannot add or remove Entities at this time!");
                default:
                    return;
            }
        }

        public void Add(IEnumerable<TickComponent> components)
        {
            foreach (TickComponent component in components)
            {
                Add(component);
            }
        }

        public void Remove(IEnumerable<TickComponent> components)
        {
            foreach (TickComponent component in components)
            {
                Remove(component);
            }
        }

        public void RemoveAll<T>() where T : TickComponent
        {
            Remove(new List<T>(GetAll<T>()));
        }

        public void Add(params TickComponent[] components)
        {
            foreach (TickComponent component in components)
            {
                Add(component);
            }
        }

        public void Remove(params TickComponent[] components)
        {
            foreach (TickComponent component in components)
            {
                Remove(component);
            }
        }

        public int Count => components.Count;

        public TickComponent this[int index]
        {
            get
            {
                if (index < 0 || index >= components.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return components[index];
            }
        }

        public IEnumerator<TickComponent> GetEnumerator()
        {
            return components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TickComponent[] ToArray()
        {
            return components.ToArray();
        }

        public void Tick()
        {
            LockMode = LockModes.Locked;
            foreach (TickComponent component in components)
            {
                if (component.Active)
                {
                    component.Tick();
                }
            }

            LockMode = LockModes.Open;
        }

        public T Get<T>() where T : TickComponent
        {
            foreach (TickComponent component in components)
            {
                if (component is T targetComponent)
                {
                    return targetComponent;
                }
            }

            return null;
        }

        public IEnumerable<T> GetAll<T>() where T : TickComponent
        {
            foreach (TickComponent component in components)
            {
                if (component is T targetComponent)
                {
                    yield return targetComponent;
                }
            }
        }
    }
}