using System.Collections;
using Lucky.Extensions;
using TMPro;
using UnityEngine;

namespace Lucky.UI.Text.TextEffect.Test
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextEffectTest : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;
        private TMP_TextInfo TextInfo => tmpText.textInfo;

        public float simulationSpeed = 20;

        private void Awake()
        {
            tmpText = GetComponent<TMP_Text>();
            tmpText.ForceMeshUpdate();
        }

        private void Start()
        {
            StartCoroutine(StartEffect());
        }

        private IEnumerator StartEffect()
        {
            while (true)
            {
                int j = 0;
                TMP_CharacterInfo charInfo = TextInfo.characterInfo[j]; // 拿到单个字符信息
                int vertexIndex = charInfo.vertexIndex;
                Debug.Log(Time.time);
                // 以时间为x, 得到y, frequency相当于x前面的常数
                Vector3 verticesOffset = new Vector3(0, Time.time, 0);
                SetVerticesOffset(vertexIndex, verticesOffset);
                // 应用
                if (Input.GetKey(KeyCode.Q))
                    tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                if (Input.GetKey(KeyCode.E))
                    tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                if (Input.GetKey(KeyCode.O))
                {
                    Color32 color = TextInfo.meshInfo[0].colors32[vertexIndex + 0];
                    color.r = 255;
                    color.g = 0;
                    color.b = 0;
                    SetColor(0, color);
                }

                if (Input.GetKey(KeyCode.P))
                    SetAlpha(0, 100);

                if (Input.GetKey(KeyCode.Return))
                    tmpText.ForceMeshUpdate();
                if (Input.GetKey(KeyCode.W))
                    tmpText.text = "ASDF";
                yield return new WaitForSeconds(1 / simulationSpeed);
            }
        }

        private void SetColor(int idx, Color32 color)
        {
            TextInfo.meshInfo[0].colors32[idx + 0] = color;
            TextInfo.meshInfo[0].colors32[idx + 1] = color;
            TextInfo.meshInfo[0].colors32[idx + 2] = color;
            TextInfo.meshInfo[0].colors32[idx + 3] = color;
        }

        private void SetAlpha(int idx, byte alpha)
        {
            for (int j = 0; j < 4; j++)
            {
                Color32 vertexColor = TextInfo.meshInfo[0].colors32[idx + j];
                TextInfo.meshInfo[0].colors32[idx + j] = vertexColor.WithA(alpha);
            }
        }

        protected void SetVerticesOffset(int idx, Vector3 offset)
        {
            TextInfo.meshInfo[0].vertices[idx + 0] += offset;
            TextInfo.meshInfo[0].vertices[idx + 1] += offset;
            TextInfo.meshInfo[0].vertices[idx + 2] += offset;
            TextInfo.meshInfo[0].vertices[idx + 3] += offset;
        }
    }
}