using Lucky.Extensions;
using UnityEngine.UI;

namespace Lucky.Console.Scripts
{
    using UnityEngine;
    using TMPro;

    public class CaretPositioner : MonoBehaviour
    {
        [SerializeField] private CustomInputField inputField;
        [SerializeField] private TMP_Text tmpText;
        [SerializeField] private RectTransform caretTransform; // 光标对象
        [SerializeField] private Image caretImage; // 光标对象
        [SerializeField] private float blinkDuration = 0.4f;
        [SerializeField] private float spaceDeltaXCompensation;
        private float blinkElapse;

        public int caretPosition;

        private float startPositionX;

        private bool dirty = false;

        private void OnEnable()
        {
            inputField.OnCaretChanged += OnValueOrCaretChanged;
        }


        private void OnDisable()
        {
            inputField.OnCaretChanged -= OnValueOrCaretChanged;
        }

        private void OnValueOrCaretChanged(string content, int caretIndex)
        {
            dirty = true;
        }

        private void Start()
        {
            startPositionX = GetStartPosition();
        }

        private float GetStartPosition()
        {
            string origText = tmpText.text;
            tmpText.text = "A";
            tmpText.ForceMeshUpdate();
            TMP_TextInfo textInfo = tmpText.textInfo;
            float posX = textInfo.characterInfo[0].bottomLeft.x;
            tmpText.text = origText;
            tmpText.ForceMeshUpdate();

            return posX;
        }

        private void Update()
        {
            UpdateBlink();
        }

        private void UpdateBlink()
        {
            blinkElapse += Time.unscaledDeltaTime;
            if (blinkElapse >= blinkDuration)
            {
                blinkElapse = 0;
                caretImage.enabled = !caretImage.enabled;
            }

            if (dirty)
            {
                dirty = false;
                caretImage.enabled = true;
                blinkElapse = 0;
            }
        }

        public void UpdateCaretVisual()
        {
            tmpText.ForceMeshUpdate();
            TMP_TextInfo textInfo = tmpText.textInfo;

            float xPos = 0;

            if (caretPosition == 0)
            {
                xPos = startPositionX;
            }
            else if (caretPosition == textInfo.characterCount)
            {
                xPos = textInfo.characterInfo[caretPosition - 1].topRight.x;
            }
            else
            {
                // xPos = textInfo.characterInfo[caretPosition].bottomLeft.x;
                var curChar = textInfo.characterInfo[caretPosition];
                var prevChar = textInfo.characterInfo[caretPosition - 1];
                xPos = curChar.bottomLeft.x;
                // 每次 tmp 相关的东西都是被 这个空格搞得很头痛 ε=( o｀ω′)ノ
                if (!curChar.isVisible)
                    xPos += spaceDeltaXCompensation;
            }

            caretTransform.anchoredPosition = caretTransform.anchoredPosition.WithX(xPos);
        }
    }
}