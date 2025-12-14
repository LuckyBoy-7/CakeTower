using Lucky.Extensions;
using UnityEngine;

namespace Lucky.Utilities
{
    public static class ColliderUtils
    {
        private static void LogNotSupportedWarning(Collider collider, Collider otherCollider)
        {
            Debug.Log($"未支持的 Collider 类型 {collider.GetType()} from {collider.gameObject.name}, {otherCollider.GetType()} from {collider.gameObject.name}");
        }

        private static void LogNotSupportedWarning(Collider collider)
        {
            Debug.LogWarning($"未支持的 Collider 类型 {collider.GetType()} from {collider.gameObject.name}");
        }

        public static bool CanCollide(Collider collider, Collider otherCollider) => (collider != null && collider.enabled && otherCollider != null && otherCollider.enabled);


        public static bool CollideCheck(Collider collider, Collider otherCollider)
        {
            if (!CanCollide(collider, otherCollider))
            {
                Debug.LogWarning("Cannot do collide check with disabled/unexisted colliders");
                return false;
            }

            if (collider is BoxCollider && otherCollider is BoxCollider)
                return collider.bounds.Intersects(otherCollider.bounds);
            LogNotSupportedWarning(collider, otherCollider);
            return false;
        }


        public static bool IsOverlappingLayer(this Collider collider, LayerMask layerMask)
        {
            if (collider is BoxCollider boxCollider)
            {
                Vector3 origin = boxCollider.bounds.center;
                Vector3 halfExtents = boxCollider.size / 2;
                return Physics.CheckBox(origin, halfExtents, Quaternion.identity, layerMask);
            }

            LogNotSupportedWarning(collider);
            return false;
        }

        /// <summary>
        /// 获取某个方向 Collider 到 Collider 的距离, 常用于吸附
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="otherCollider"></param>
        /// <param name="normalizedOneAxisDir"></param>
        /// <returns></returns>
        public static float GetDistToOtherColliderByOneAxis(this Collider collider, Collider otherCollider, Vector3 normalizedOneAxisDir)
        {
            if (collider is BoxCollider boxCollider && otherCollider is BoxCollider otherBoxCollider)
            {
                var selector = normalizedOneAxisDir.GetAxisValueSelector();
                float selfMin = selector(boxCollider.bounds.min);
                float selfMax = selector(boxCollider.bounds.max);

                float otherMin = selector(otherBoxCollider.bounds.min);
                float otherMax = selector(otherBoxCollider.bounds.max);

                if (selfMin > otherMax)
                    return selfMin - otherMax;
                if (selfMax < otherMin)
                    return otherMin - selfMax;
                return 0;
            }

            Debug.LogWarning("GetDistToOtherColliderInOneAxis 接收到了暂未支持的 collider 类型!");
            return 0;
        }
    }
}