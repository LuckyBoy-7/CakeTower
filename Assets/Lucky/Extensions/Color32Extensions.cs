using UnityEngine;

namespace Lucky.Extensions
{
    public static class Color32Extensions
    {
        public static Color32 WithA(this Color32 orig, byte x)
        {
            orig.a = x;
            return orig;
        }
    }
}