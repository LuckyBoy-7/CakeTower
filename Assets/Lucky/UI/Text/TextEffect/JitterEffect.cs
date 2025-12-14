using Lucky.Extensions;
using TMPro;
using UnityEngine;

namespace Lucky.UI.Text.TextEffect
{
    public class JitterEffect : TextEffectBase
    {
        private float jitterAmount = 2f;

        public override TextEffectBase CreateInstance() => new JitterEffect { tmpText = tmpText };

        public override string GetPattern() => @"<jitter\b([\s\S]*?)>([\s\S]*?)</jitter>";

        public override void TakeEffect()
        {
            float jitterAmount = args.GetFloat("jitterAmount", this.jitterAmount);

            Vector3 GetRandomJitterOffset()
            {
                float mult = 100;
                float xOffset = Random.Range((int)-jitterAmount * mult, (int)jitterAmount * mult);
                float yOffset = Random.Range((int)-jitterAmount * mult, (int)jitterAmount * mult);
                return new Vector3(xOffset, yOffset) / 100f;
            }

            Vector3 preShakeOffset0 = GetRandomJitterOffset();
            Vector3 preShakeOffset1 = GetRandomJitterOffset();
            Vector3 preShakeOffset2 = GetRandomJitterOffset();
            Vector3 preShakeOffset3 = GetRandomJitterOffset();
            for (var j = start; j < start + length; j++) // 遍历每个字符
            {
                TMP_CharacterInfo charInfo = TextInfo.characterInfo[j]; // 拿到单个字符信息
                if (charInfo.character == ' ')
                    continue;


                int vertexIndex = charInfo.vertexIndex;
                if (args.ContainsKey("share") && args["share"] == "true")
                {
                    TextInfo.meshInfo[0].vertices[vertexIndex + 0] += preShakeOffset0;
                    TextInfo.meshInfo[0].vertices[vertexIndex + 1] += preShakeOffset1;
                    TextInfo.meshInfo[0].vertices[vertexIndex + 2] += preShakeOffset2;
                    TextInfo.meshInfo[0].vertices[vertexIndex + 3] += preShakeOffset3;
                }
                else
                {
                    TextInfo.meshInfo[0].vertices[vertexIndex + 0] += GetRandomJitterOffset();
                    TextInfo.meshInfo[0].vertices[vertexIndex + 1] += GetRandomJitterOffset();
                    TextInfo.meshInfo[0].vertices[vertexIndex + 2] += GetRandomJitterOffset();
                    TextInfo.meshInfo[0].vertices[vertexIndex + 3] += GetRandomJitterOffset();
                }
            }
        }
    }
}