using System.Collections.Generic;

namespace Lucky.Utilities
{
    public class FlagSystem
    {
        private HashSet<string> flags = new();

        public void SetFlag(string flag, bool on = true)
        {
            if (on)
                flags.Add(flag);
            else
                flags.Remove(flag);
        }

        public bool GetFlag(string flag)
        {
            return flags.Contains(flag);
        }
    }
}