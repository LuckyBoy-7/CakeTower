using System;
using UnityEngine;

namespace Lucky.Utilities
{
    [Serializable]
    public class Timer
    {
        [SerializeField] private float timer = -1;
        [SerializeField] private float duration;
        private Action callback;

        public Timer(float duration, Action callback = null)
        {
            this.duration = duration;
            this.callback = callback;
        }

        public void Consume()
        {
            timer = -1;
        }

        public void Restart()
        {
            timer = duration;
        }

        public void Restart(float targetDuration)
        {
            timer = targetDuration;
        }

        /// <summary>
        /// 有的时候我们希望在定时器时间之内做某些事情
        /// </summary>
        /// <returns></returns>
        public bool InTime => timer > 0;

        public bool Over => !InTime;
        public float Value => timer;


        /// <summary>
        /// 有的时候我们希望间隔一定时间触发一个事件
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public bool Update(float deltaTime)
        {
            if (duration <= 0f || timer == -1)
                return false;

            timer -= deltaTime;
            if (timer <= 0)
            {
                callback?.Invoke();
                timer = -1;
                return true;
            }

            return false;
        }


        public static float GetTime(bool realtime = false) => realtime ? Time.realtimeSinceStartup : Time.time;
        public static float DeltaTime(bool realtime = false) => realtime ? Time.unscaledDeltaTime : Time.deltaTime;
        public static float FixedDeltaTime(bool realtime = false) => realtime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;

        /// 每一个interval开始的时候触发一次, 相当于 true false false false ... true false false false ... 
        public static bool OnInterval(float interval, bool realtime = false)
        {
            return (int)((GetTime(realtime) - (double)FixedDeltaTime(realtime)) / interval) < (int)((double)GetTime(realtime) / interval);
        }

        /// 获取按interval交替的bool, 相当于 true true true ... false false false ... true true true
        public static bool BetweenInterval(float interval, bool realtime = false)
        {
            return GetTime(realtime) % (interval * 2f) > interval;
        }

        /// 同上, 只不过这里时间流逝是跟着val的 
        public static bool BetweenInterval(float val, float interval)
        {
            return val % (interval * 2f) > interval;
        }
    }
}