using System.Collections.Generic;
using Lucky.Collections;
using Lucky.Extensions;
using UnityEngine;

namespace Lucky.Utilities
{
    public static class LogUtils
    {
        private static DefaultDict<string, float> lastLogTimestamps = new DefaultDict<string, float>(() => 0f);

        public static void Initialize()
        {
            lastLogTimestamps.Clear();
        }

        public static void Log(string message, string id = "", float interval = 0)
        {
            if (interval == 0)
            {
                Debug.Log(message);
                return;
            }

            string key = string.IsNullOrEmpty(id) ? message : id;

            float currentTime = Time.realtimeSinceStartup;
            if (currentTime - lastLogTimestamps[key] >= interval)
            {
                Debug.Log(message);
                lastLogTimestamps[key] = currentTime;
            }
        }
    }
}