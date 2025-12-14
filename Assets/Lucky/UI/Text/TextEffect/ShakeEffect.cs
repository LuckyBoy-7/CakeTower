using Lucky.Extensions;
using TMPro;
using UnityEngine;

namespace Lucky.UI.Text.TextEffect
{
    public class ShakeEffect : TextEffectBase
    {
        private float shakeAmount = 2f;

        public override TextEffectBase CreateInstance() => new ShakeEffect { tmpText = tmpText };

        public override string GetPattern() => @"<shake\b([\s\S]*?)>([\s\S]*?)</shake>";

        public override void TakeEffect()
        {
            float shakeAmount = args.GetFloat("shakeAmount", this.shakeAmount);

            Vector3 GetRandomShakeOffset()
            {
                float mult = 100;
                float xOffset = Random.Range((int)-shakeAmount * mult, (int)shakeAmount * mult);
                float yOffset = Random.Range((int)-shakeAmount * mult, (int)shakeAmount * mult);
                return new Vector3(xOffset, yOffset) / 100f;
            }

            Vector3 preShakeOffset = GetRandomShakeOffset();
            for (var j = start; j < start + length; j++) // 遍历每个字符
            {
                TMP_CharacterInfo charInfo = TextInfo.characterInfo[j]; // 拿到单个字符信息
                if (charInfo.character == ' ')
                    continue;


                int vertexIndex = charInfo.vertexIndex;
                Vector3 verticesOffset = args.ContainsKey("share") && args["share"] == "true"
                    ? preShakeOffset
                    : GetRandomShakeOffset();
                SetVerticesOffset(vertexIndex, verticesOffset);
            }
        }
    }
}