using Lucky.Extensions;
using TMPro;
using UnityEngine;

namespace Lucky.UI.Text.TextEffect
{
    public class FloatEffect : TextEffectBase
    {
        public float frequency = 10f;
        public float magnitude = 5f;

        public override TextEffectBase CreateInstance() => new FloatEffect { tmpText = tmpText };

        public override string GetPattern() => @"<float\b([\s\S]*?)>([\s\S]*?)</float>";

        public override void TakeEffect()
        {
            float frequency = args.GetFloat("frequency", this.frequency);
            float magnitude = args.GetFloat("magnitude", this.magnitude);
            for (var j = start; j < start + length; j++) // 遍历每个字符
            {
                TMP_CharacterInfo charInfo = TextInfo.characterInfo[j]; // 拿到单个字符信息
                // 有个逆天的问题不知道为什么，就是好像有空格的部分施加效果是无效的，然后多的会转移到第一个字符上？
                // tmp的坑我已经受够了(╯▔皿▔)╯
                if (charInfo.character == ' ')
                    continue;

                int vertexIndex = charInfo.vertexIndex;
                // 以时间为x, 得到y, frequency相当于x前面的常数
                Vector3 verticesOffset = new Vector3(0,
                    Mathf.Sin(frequency * Time.time + j) * magnitude, 0);
                SetVerticesOffset(vertexIndex, verticesOffset);
            }
        }
    }
}