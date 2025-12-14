using UnityEngine;

namespace Lucky.Extensions
{
    public static class TransformExtensions
    {
        public static void SetPositionX(this Transform orig, float x) => orig.position = orig.position.WithX(x);
        public static void SetPositionY(this Transform orig, float y) => orig.position = orig.position.WithY(y);
        public static void SetPositionZ(this Transform orig, float z) => orig.position = orig.position.WithZ(z);
        
        public static void SetLocalPositionX(this Transform orig, float x) => orig.localPosition = orig.localPosition.WithX(x);
        public static void SetLocalPositionY(this Transform orig, float y) => orig.localPosition = orig.localPosition.WithY(y);
        public static void SetLocalPositionZ(this Transform orig, float z) => orig.localPosition = orig.localPosition.WithZ(z);
        
        public static void AddPositionX(this Transform orig, float x) => orig.position = orig.position.WithX(orig.position.x + x);
        public static void AddPositionY(this Transform orig, float y) => orig.position = orig.position.WithY(orig.position.y + y);
        public static void AddPositionZ(this Transform orig, float z) => orig.position = orig.position.WithZ(orig.position.z + z);
        
        // 2d eulerAngles就够了
        public static void SetRotationX(this Transform orig, float x) => orig.eulerAngles = orig.eulerAngles.WithX(x);
        public static void SetRotationY(this Transform orig, float y) => orig.eulerAngles = orig.eulerAngles.WithY(y);
        public static void SetRotationZ(this Transform orig, float z) => orig.eulerAngles = orig.eulerAngles.WithZ(z);
        
        public static void SetScaleX(this Transform orig, float x) => orig.localScale = orig.localScale.WithX(x);
        public static void SetScaleY(this Transform orig, float y) => orig.localScale = orig.localScale.WithY(y);
        public static void SetScaleZ(this Transform orig, float z) => orig.localScale = orig.localScale.WithZ(z);
        public static Vector3 ScreenPosition(this Transform orig) => Camera.main.WorldToScreenPoint(orig.position);
        public static void SetScreenPosition(this Transform orig, Vector2 screenPosition) => orig.position = Camera.main.ScreenToWorldPoint(screenPosition);
        
        
        public static float Dist(this Transform orig, GameObject go) => Vector2.Distance(orig.position, go.transform.position);
        public static float Dist(this Transform orig, Transform other) => Vector2.Distance(orig.position, other.position);
        public static Vector2 Dir(this Transform orig, GameObject go) => go.transform.position - orig.position;
        public static Vector2 Dir(this Transform orig, Transform other) => other.position - orig.position;

        /// <summary>
        /// 使 transform 向某个方向移动固定距离并消耗传进来的距离
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="maxMovement"></param>
        /// <param name="distance"></param>
        public static void MoveAlongWithCost(this Transform orig, ref Vector3 maxMovement, float distance)
        {
            Vector3 nextMovement = Vector3.MoveTowards(maxMovement, Vector3.zero, distance);
            orig.transform.position += maxMovement - nextMovement;
            maxMovement = nextMovement;
        }
    }
}