using UnityEngine;

namespace Lucky.Utilities
{
    public static class InstantiateUtils
    {
        /// <summary>
        /// 公共的子物体初始化逻辑
        /// </summary>
        private static void SetupChild(Transform child, Transform parent)
        {
            child.SetParent(parent);
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
        }

        public static T AddChildWithComponent<T>(this GameObject gameObject) where T : Component
        {
            GameObject child = new GameObject($"{gameObject.name}_{typeof(T).Name}");
            SetupChild(child.transform, gameObject.transform);
            return child.AddComponent<T>();
        }
        

        public static T AddChildPrefab<T>(this GameObject gameObject, T prefab) where T : Object
        {
            if (!prefab)
                return null;

            T t = Object.Instantiate(prefab);
            if (t is Component com)
            {
                SetupChild(com.transform, gameObject.transform);
            }
            else if (t is GameObject go)
            {
                SetupChild(go.transform, gameObject.transform);
            }

            else
            {
                Debug.LogError($"AddChildPrefab 不支持的类型: {typeof(T).Name}");
            }

            return t;
        }
    }
}