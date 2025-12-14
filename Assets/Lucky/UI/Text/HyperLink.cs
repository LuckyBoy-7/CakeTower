using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Interactive;
using Lucky.UI.Text.TextEffect;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Lucky.UI.Text
{
    /// <summary>
    /// &lt;link=123&gt;&lt;/link&gt;
    /// </summary>
    public class HyperLink : MonoBehaviour
    {
        private TextController textController;
        private TextEffectController textEffectController;
        public Color linkColor = Color.red;
        public Color pressedColor = Color.green;
        public Color visitedColor = Color.blue;
        private TMP_Text text;
        public Texture2D HandTexture;
        [ShowInInspector] private HashSet<string> linkVisited = new();
        [SerializeField] private string pressedId = "";
        [SerializeField] private string hoverId = "";

        public bool CanInteract => textController == null || !textController.isTalking;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            textController = GetComponent<TextController>();
            textEffectController = GetComponent<TextEffectController>();
        }

        public void ResetLink()
        {
            linkVisited.Clear();
        }

        public void Update()
        {
            if (!CanInteract)
                return;

            int hoveringLinkIndex = TMP_TextUtilities.FindIntersectingLink(text, GameCursor.MouseScreenPosition, null);
            if (hoveringLinkIndex != -1)
            {
                hoverId = text.textInfo.linkInfo[hoveringLinkIndex].GetLinkID();
                if (Input.GetMouseButtonDown(0))
                    pressedId = hoverId;
            }
            else
            {
                hoverId = "";
            }

            if (Input.GetMouseButtonUp(0) && pressedId != "")
            {
                linkVisited.Add(pressedId);
                pressedId = "";
            }

            // 只管鼠标样式
            if ((pressedId != "" || hoverId != "") && HandTexture != null)
                Cursor.SetCursor(HandTexture, new Vector2(10, 0), CursorMode.Auto);
            else
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            if (textEffectController == null)
                ApplyLinkColors();
        }

        // 关键：直接操作顶点颜色，不改文本！
        public void ApplyLinkColors()
        {
            foreach (var linkInfo in text.textInfo.linkInfo)
            {
                string linkId = linkInfo.GetLinkID();

                Color32 color = GetLinkColorByID(linkId);

                // 直接给这段文字的所有字符染色
                for (int i = linkInfo.linkTextfirstCharacterIndex; i <= linkInfo.linkTextLength + linkInfo.linkTextfirstCharacterIndex - 1; i++)
                {
                    var charInfo = text.textInfo.characterInfo[i];
                    if (!charInfo.isVisible)
                        continue;

                    int vertexIndex = charInfo.vertexIndex;
                    int materialIndex = charInfo.materialReferenceIndex;

                    for (int j = 0; j < 4; j++)
                    {
                        Color32 vertexColor = text.textInfo.meshInfo[materialIndex].colors32[vertexIndex + j];
                        text.textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = color.WithA(vertexColor.a);
                    }
                }

                if (textEffectController == null)
                    text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }
        }

        private Color GetLinkColorByID(string linkId)
        {
            // 决定这个链接用什么颜色
            Color color;
            if (linkId == pressedId)
                color = pressedColor;
            else if (linkVisited.Contains(linkId))
                color = visitedColor;
            else
                color = linkColor;
            return color;
        }
    }
}