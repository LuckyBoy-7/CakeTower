using System;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 WithX(this Vector3 orig, float x)
        {
            orig.x = x;
            return orig;
        }

        public static Vector3 WithY(this Vector3 orig, float y)
        {
            orig.y = y;
            return orig;
        }

        public static Vector3 WithZ(this Vector3 orig, float z)
        {
            orig.z = z;
            return orig;
        }

        public static Vector3 Sign(this Vector3 orig)
        {
            return new Vector3(MathUtils.Sign(orig.x), MathUtils.Sign(orig.y), MathUtils.Sign(orig.z));
        }

        public static void Deconstruct(this Vector3 vector, out float x, out float y, out float z)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public static Vector3 Multiply(this Vector3 orig, Vector3 other)
        {
            return new Vector3(orig.x * other.x, orig.y * other.y, orig.z * other.z);
        }

        public static Vector3 X(this Vector3 orig)
        {
            return new Vector3(orig.x, 0, 0);
        }

        public static Vector3 Y(this Vector3 orig)
        {
            return new Vector3(0, orig.y, 0);
        }

        public static Vector3 Z(this Vector3 orig)
        {
            return new Vector3(0, 0, orig.z);
        }

        public static float XValue(this Vector3 orig)
        {
            return orig.x;
        }

        public static float YValue(this Vector3 orig)
        {
            return orig.y;
        }

        public static float ZValue(this Vector3 orig)
        {
            return orig.z;
        }

        /// <summary>
        /// 以当前向量作为方向, 返回一个函数, 该函数可以选择性地抽取该方向的分量
        /// </summary>
        /// <param name="orig"></param>
        /// <returns></returns>
        public static Func<Vector3, Vector3> GetAxisSelector(this Vector3 orig)
        {
            if (orig.x != 0)
                return X;
            if (orig.y != 0)
                return Y;
            return Z;
        }

        /// <summary>
        /// 以当前向量作为方向, 返回一个函数, 该函数可以选择性地抽取该方向的分量的值
        /// </summary>
        /// <param name="orig"></param>
        /// <returns></returns>
        public static Func<Vector3, float> GetAxisValueSelector(this Vector3 orig)
        {
            if (orig.x != 0)
                return XValue;
            if (orig.y != 0)
                return YValue;
            return ZValue;
        }

        public static Vector3 XZ(this Vector3 orig)
        {
            return new Vector3(orig.x, 0, orig.z);
        }

        public static Vector3 WithXZ(this Vector3 orig, Vector3 other)
        {
            return new Vector3(other.x, orig.y, other.z);
        }


        /// <summary>
        /// 用传进来的 vector3 的非零部分覆盖当前 vector3
        /// </summary>
        public static Vector3 OverlapWithNonZeroValue(this Vector3 orig, Vector3 value)
        {
            if (value.x != 0)
                orig.x = value.x;
            if (value.y != 0)
                orig.y = value.y;
            if (value.z != 0)
                orig.z = value.z;
            return orig;
        }

        public struct BaseVectors
        {
            public Vector3 forward;
            public Vector3 right;
            public Vector3 up;
        }

        /// <summary>
        /// 用传进来的 vector3 的 forward 方向朝向新的 forward 方向(这里是朝向前), 并返回新的 3 个基向量
        /// </summary>
        public static BaseVectors GetBaseVectorsAfterLookAhead(this Vector3 orig)
        {
            return orig.GetBaseVectorsAfterLookAt(orig.XZ().normalized);
        }

        /// <summary>
        /// 用传进来的 vector3 的 forward 方向朝向新的 forward 方向, 并返回新的 3 个基向量
        /// </summary>
        public static BaseVectors GetBaseVectorsAfterLookAt(this Vector3 orig, Vector3 newForward)
        {
            Quaternion rot = Quaternion.LookRotation(newForward, Vector3.up);

            // 新的 3 个基向量：
            Vector3 forward = rot * Vector3.forward;
            Vector3 right = rot * Vector3.right;
            Vector3 up = rot * Vector3.up;
            return new BaseVectors()
            {
                forward = forward,
                right = right,
                up = up
            };
        }
    }
}