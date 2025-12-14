using System.Collections.Generic;

namespace Lucky.Extensions
{
    public static class IDictionaryExtensions
    {
        public static void Merge<TKey, TVal>(this IDictionary<TKey, TVal> orig, IDictionary<TKey, TVal> other)
        {
            foreach (var (key, value) in other)
            {
                orig[key] = value;
            }
        }

        public delegate bool TryParseDelegate<T>(string valueString, out T result);

        public static T GetValue<T>(
            this IDictionary<string, string> dict,
            string key,
            T defaultValue,
            TryParseDelegate<T> tryParse)
        {
            if (dict.TryGetValue(key, out string value) && tryParse(value, out T result))
                return result;
            return defaultValue;
        }

        public static int GetInt(this IDictionary<string, string> orig, string key, int defaultValue = 0) => orig.GetValue(key, defaultValue, int.TryParse);
        public static float GetFloat(this IDictionary<string, string> orig, string key, float defaultValue = 0) => orig.GetValue(key, defaultValue, float.TryParse);
    }
}