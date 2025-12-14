using System.Collections.Generic;

namespace Lucky.Utilities
{
    public class HistoryTraceback<T>
    {
        private List<T> history = new List<T>();
        private int index = 0;

        public void Add(T item)
        {
            history.Add(item);
            ResetIndex();
        }

        public void ResetIndex()
        {
            index = history.Count;
        }

        public bool TryGetItem(out T item)
        {
            if (history.Count == 0 || index < 0 || index >= history.Count)
            {
                item = default;
                return false;
            }

            item = history[index];
            return true;
        }


        public void Forward()
        {
            if (history.Count == 0)
                return;
            index = MathUtils.Mod(index + 1, history.Count + 1);
        }

        public void Backward()
        {
            if (history.Count == 0)
                return;

            index = MathUtils.Mod(index - 1, history.Count + 1);
        }

        public void Reset()
        {
            history.Clear();
            ResetIndex();
        }
    }
}