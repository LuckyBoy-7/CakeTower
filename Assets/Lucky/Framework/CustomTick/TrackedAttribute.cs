using System;

namespace Lucky.Framework.CustomTick
{
    public class TrackedAttribute : Attribute
    {
        public bool AlsoInherited { get; }

        public TrackedAttribute(bool alsoInherited = false)
        {
            AlsoInherited = alsoInherited;
        }
    }
}