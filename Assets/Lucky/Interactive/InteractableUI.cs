using Sirenix.OdinInspector;
using UnityEngine;

namespace Lucky.Interactive
{
    public class InteractableUI : InteractableUIBase
    {
        private enum InteractableUITypes
        {
            Screen,
            World
        }

        private InteractableUITypes interactableUIType = InteractableUITypes.Screen;

        [FoldoutGroup("Interactable"), ShowInInspector]
        public bool ignoreBounds;


        // 因为 interactable 往往是被实例化的那个, 所以如果放 Awake 里会导致找不到 canvas
        protected virtual void Start()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
                interactableUIType = InteractableUITypes.World;
        }

        public override bool CursorInBounds()
        {
            if (ignoreBounds)
                return true;
            return GetWorldRect(RectTransform).Contains(
                interactableUIType == InteractableUITypes.Screen ?
                // 此时相对屏幕坐标系, canvas的宽高正好是屏幕的宽高, 所以正常情况canvas大小能和MouseScreenPositionu范围对上
                GameCursor.MouseScreenPosition :
                // 此时相对世界坐标系
                GameCursor.MouseWorldPosition);
        }

        protected void OnEnable()
        {
            GameCursor.Instance?.RegisterInteractable<InteractableUI>(this);
        }

        protected void OnDisable()
        {
            GameCursor.Instance?.UnregisterInteractable<InteractableUI>(this);
        }
    }
}