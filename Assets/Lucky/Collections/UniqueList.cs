using System.Collections;
using System.Collections.Generic;

namespace Lucky.Collections
{
    /// <summary>
    /// 保证 List 元素不重复, 有顺序, 且不会因为元素的增删改查而中断遍历
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class UniqueList<T> : IEnumerable<T>
    {
        private readonly HashSet<T> hashSet = new HashSet<T>();

        private readonly List<T> list = new List<T>();

        private bool dirty;

        private bool pendingClear;

        private int reservationCount;

        public List<T> List
        {
            get
            {
                UpdateList();
                return list;
            }
        }

        public int Count => hashSet.Count;

        public void ReserveListUsage()
        {
            reservationCount++;
        }

        public void ReleaseListUsage()
        {
            reservationCount--;
            if (reservationCount <= 0)
            {
                reservationCount = 0;
                if (pendingClear)
                {
                    list.Clear();
                    dirty = false;
                    pendingClear = false;
                }
            }
        }

        public void UpdateList()
        {
            if (dirty)
            {
                dirty = false;
                list.Clear();
                list.AddRange(hashSet);
            }
        }

        public bool Add(T element)
        {
            if (hashSet.Add(element))
            {
                dirty = true;
                return true;
            }

            return false;
        }

        public bool Remove(T element)
        {
            if (hashSet.Remove(element))
            {
                dirty = true;
                if (hashSet.Count == 0)
                {
                    if (reservationCount == 0)
                    {
                        list.Clear();
                        dirty = false;
                    }
                    else
                    {
                        pendingClear = true;
                    }
                }

                return true;
            }

            return false;
        }

        public void Clear()
        {
            hashSet.Clear();
            if (reservationCount == 0)
            {
                list.Clear();
                dirty = false;
                return;
            }

            dirty = true;
            pendingClear = true;
        }

        public void FullClear()
        {
            hashSet.Clear();
            list.Clear();
            dirty = false;
            pendingClear = false;
            reservationCount = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T value in List)
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}