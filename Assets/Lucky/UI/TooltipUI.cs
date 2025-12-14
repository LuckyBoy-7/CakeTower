using Lucky.Framework;
using Lucky.Interactive;
using UnityEngine;

namespace Lucky.UI
{
    public class TooltipUI : Singleton<TooltipUI>
    {
        public float diagonalDelta;
        public RectTransform panelRectTransform;
        [SerializeField] private bool isShowing;

        public void FixedUpdate()
        {
            if (isShowing)
                UpdatePos();
        }


        // 设置锚点为左上角（然后加上偏移），如果超出屏幕范围
        // 则把锚点设置到右上角，并施加偏移
        private void UpdatePos()
        {
            Vector2 mousePos = GameCursor.MouseWorldPosition;
            // 默认pivot为左上角
            panelRectTransform.pivot = new Vector2(0, 1);
            if (mousePos.x + panelRectTransform.sizeDelta.x + diagonalDelta > (float)Screen.width / 2)
                // pivot变为右上角
                panelRectTransform.pivot += new Vector2(1, 0);
            if (mousePos.y - panelRectTransform.sizeDelta.y - diagonalDelta < -(float)Screen.height / 2)
                panelRectTransform.pivot -= new Vector2(0, 1);

            // 0 1 -> 1 -1
            // 1 1 -> -1 -1
            // 0 0 -> 1 1
            // 1 0 -> -1 1
            // x -> -(x * 2 - 1)
            panelRectTransform.position = mousePos + new Vector2(diagonalDelta, diagonalDelta) * -(panelRectTransform.pivot * 2 - Vector2.one);
        }

        /// <summary>
        /// 到时候show里写参数给对应UI赋值就行
        /// </summary>
        public void Show()
        {
            isShowing = true;
            panelRectTransform.gameObject.SetActive(true);

            UpdatePos();
        }

        public void Hide()
        {
            isShowing = false;
            panelRectTransform.gameObject.SetActive(false);
        }
    }
}