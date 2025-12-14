using System;
using System.Collections;
using UnityEngine;

namespace Lucky.Extensions
{
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// 动态的计时，并且会随着gameObject关闭或是StopAllCoroutines而停止，而invokeRepeating好像会一直执行，且不动态
        /// </summary>
        /// <param name="orig">调用对象</param>
        /// <param name="callback">回调函数</param>
        /// <param name="interval">调用时间间隔</param>
        /// <param name="invokeOnStart">是马上调用还是隔 interval 后再调用</param>
        /// <param name="scaledTime">是否受时间缩放影响</param>
        /// <param name="onlyOnce">是否就执行一次</param>
        public static void CreateFuncTimer(
            this MonoBehaviour orig,
            Action callback,
            Func<float> interval,
            bool onlyOnce = false,
            bool invokeOnStart = true,
            bool scaledTime = true
        )
        {
            float elapse = invokeOnStart ? interval() : 0;
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                while (true)
                {
                    elapse += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
                    if (elapse >= interval())
                    {
                        elapse -= interval();
                        callback?.Invoke();
                        if (onlyOnce)
                            yield break;
                    }

                    yield return null;
                }
            }
        }

        public static void WaitForOneFrameToExecution(
            this MonoBehaviour orig,
            Action callback
        )
        {
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                yield return null;
                callback?.Invoke();
            }
        }
    }
}