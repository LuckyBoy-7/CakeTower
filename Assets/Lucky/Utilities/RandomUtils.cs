using UnityEngine;
using Random = System.Random;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Utilities
{
    public static class RandomUtils
    {
        public static readonly Random Random = new();

        /// <summary>
        /// 生成一个以pos为圆心, radius为半径圆中的随机点(常用在patrol)
        /// </summary>
        /// <param name="pos">圆心</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public static Vector3 RandomPosAroundPoint(Vector3 pos, float radius)
        {
            return pos + InsideUnitCircle * radius;
        }

        /// <summary>
        /// 生成单位圆内均匀分布的随机点
        /// </summary>
        public static Vector3 InsideUnitCircle
        {
            get
            {
                // // 拒绝采样
                // float x, y;
                // do
                // {
                //     x = NextFloat();
                //     y = NextFloat();
                // } while (x * x + y * y > 1);
                //
                // return new Vector2(x - 0.5f, y - 0.5f);

                // 
                float radians = NextFloat(PI(2));
                float length = Sqrt(NextFloat());
                return RadiansToVector(radians, length);
            }
        }

        /// <summary>
        /// 随机一个[0, 1)的float
        /// </summary>
        public static float NextFloat()
        {
            return (float)Random.NextDouble();
        }

        /// <summary>
        /// 随机一个[0, 1) * val的float
        /// </summary>
        public static float NextFloat(float mul)
        {
            return NextFloat() * mul;
        }

        public static float NextFloat(int mul) => NextFloat((float)mul);


        /// <summary>
        /// 随机一个[0 ~ 2pi] * mul 的radians
        /// </summary>
        public static float NextRadians(float mul = 1)
        {
            return NextFloat() * Mathf.PI * 2 * mul;
        }

        /// <summary>
        /// 随机一个[-1, 1) * val的float
        /// </summary>
        public static float NextSignedFloat(float mul = 1) => (NextFloat() * 2 - 1) * mul;


        /// <summary>
        /// 随机一个[0, max)的int
        /// </summary>
        public static int Range(int max) => Range(0, max);

        /// <summary>
        /// 随机一个[min, max)的int
        /// </summary>
        public static int Range(int min, int max)
        {
            return min + Random.Next(max - min);
        }

        /// <summary>
        /// 随机一个[min, max)的float
        /// </summary>
        public static float Range(float min, float max)
        {
            return min + NextFloat(max - min);
        }

        /// <summary>
        /// 在传入的数组中抽一个值
        /// </summary>
        public static T Choose<T>(params T[] choices)
        {
            return choices[Random.Next(choices.Length)];
        }

        /// <summary>
        /// 随机一个[min Vector2, max Vector2)之间的Vector2
        /// </summary>
        public static Vector2 Range(Vector2 min, Vector2 max)
        {
            return min + new Vector2(NextFloat(max.x - min.x), NextFloat(max.y - min.y));
        }

        public static int Random01() => Choose(0, 1);

        public static Vector2 RandomPointInRect(Rect rect)
        {
            float x = Range(rect.x, rect.xMax);
            float y = Range(rect.y, rect.yMax);
            return new Vector2(x, y);
        }

        #region Noise

        public static float RandomNoise(Vector2 seed)
        {
            return Frac(Sin(Vector2.Dot(seed * Floor(30), new Vector2(127.1f, 311.7f))) * 43758.5453123f);
            // return Frac(Sin(Vector2.Dot(seed * Floor(NextFloat() * 30), new Vector2(127.1f, 311.7f))) * 43758.5453123f);
        }

        public static float RandomNoise(float x, float y) => RandomNoise(new Vector2(x, y));

        public static float RandomNoise(float seed)
        {
            return RandomNoise(new Vector2(seed, 1));
        }

        #endregion
    }
}