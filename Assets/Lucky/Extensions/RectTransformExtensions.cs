using UnityEngine;

namespace Lucky.Extensions
{
    public static class RectTransformExtensions
    {
        public static void SetAnchoredPositionX(this RectTransform orig, float x) => orig.anchoredPosition = orig.anchoredPosition.WithX(x);
        public static void SetAnchoredPositionY(this RectTransform orig, float y) => orig.anchoredPosition = orig.anchoredPosition.WithY(y);
        public static void SetAnchoredPositionZ(this RectTransform orig, float z) => orig.anchoredPosition = orig.anchoredPosition.WithZ(z);

        public static void AddAnchoredPositionX(this RectTransform orig, float x) =>
            orig.anchoredPosition = orig.anchoredPosition.WithX(orig.anchoredPosition.x + x);

        public static void AddAnchoredPositionY(this RectTransform orig, float y) =>
            orig.anchoredPosition = orig.anchoredPosition.WithY(orig.anchoredPosition.y + y);

        public static void SetAnchor(this RectTransform orig, Vector2 pos) => orig.anchorMin = orig.anchorMax = pos;
        public static void SetPivot(this RectTransform orig, Vector2 pos) => orig.pivot = pos;

        public static Rect GetScreenSpaceRect(this RectTransform rectTransform)
        {
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();

            // 获取四个角的世界坐标
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // 确定相机
            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            // 转换角点到屏幕坐标
            Vector2 bottomLeft, topRight;

            if (cam == null)
            {
                // Overlay 模式
                bottomLeft = corners[0];
                topRight = corners[2];
            }
            else
            {
                // Camera 或 World Space 模式
                bottomLeft = cam.WorldToScreenPoint(corners[0]);
                topRight = cam.WorldToScreenPoint(corners[2]);
            }

            // 创建 Rect (x, y 是左下角坐标, width, height 是宽高)
            Rect screenRect = new Rect(
                bottomLeft.x,
                bottomLeft.y,
                topRight.x - bottomLeft.x,
                topRight.y - bottomLeft.y
            );

            return screenRect;
        }
    }
}