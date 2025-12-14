using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lucky.Collections;
using Lucky.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Lucky.UI.Text.TextEffect
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextEffectController : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;
        public HyperLink hyperLink;
        private TMP_TextInfo TextInfo => tmpText.textInfo;

        public float simulationSpeed = 20;

        public int showCharNum;

        // 方便调试
        [Multiline, SerializeField] private string rawContent = "";
        private string preRawContent = "";
        [Multiline, SerializeField] private string tmpParsedContent = "";
        [Multiline, SerializeField] public string finalParsedContent = "";

        public string RawContent
        {
            get => rawContent;
            set
            {
                preRawContent = rawContent = value;
                charPosToEventInfo.Clear();
                currentEffects.Clear();
                ParseString(); // 设置的时候就要parse，不然update执行顺序会滞后，逻辑错误
                AdjustCharactersVisibility();
            }
        }


        /// 对应字符位置拿到单个标签的信息如[speed=11/], [delay=0.5/] (这里把尖括号换成中括号了, 不然识别不出来)
        /// 搭配 TextController 食用
        [ShowInInspector] public DefaultDict<int, Dictionary<string, string>> charPosToEventInfo = new(() => new());

        private List<TextEffectBase> currentEffects = new();
        private List<TextEffectBase> textEffects = new();

        void Awake()
        {
            tmpText = GetComponent<TMP_Text>();
            foreach (TextEffectBase textEffect in new List<TextEffectBase>
                     {
                         new SpeedEffect(),
                         new DelayEffect(),
                         new ShakeEffect(),
                         new FloatEffect(),
                         new JumpEffect(),
                         new JitterEffect(),
                     })
            {
                textEffects.Add(textEffect);
                textEffect.tmpText = tmpText;
            }
        }

        private void Start()
        {
            RawContent = rawContent;
            StartCoroutine(StartEffect());
        }

        private void SetText(string content)
        {
            tmpText.text = content;
            tmpText.ForceMeshUpdate(); // 不这么做 AdjustCharactersVisibility 就拿不到正确网格了
        }

        private IEnumerator StartEffect()
        {
            while (true)
            {
                if (RawContent != preRawContent) // 这样直接改 rawContent 就能看效果了, 方便测试
                {
                    RawContent = rawContent;
                    preRawContent = rawContent;
                }

                // 刷新
                // 对文本中的每一段应用效果
                foreach (var effect in currentEffects)
                    effect.TakeEffect();
                // 调整字符显示个数
                AdjustCharactersVisibility();

                // 应用
                tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

                yield return new WaitForSeconds(1 / simulationSpeed);
            }
        }

        private void AdjustCharactersVisibility()
        {
            // 不知道为什么空格就是有bug
            // 已解决: https://discussions.unity.com/t/first-character-switches-with-blank-space-character/926744
            // Solved by skipping blank spaces. Blank spaces were somehow read as default and as such vertices at index 0 were modified.    
            // 简单来说空格好像会影响第一个字符, 所以跳过就好
            // 管理字符显隐
            // 如果要实现那种字符从左到右淡入的效果, 那么可以让i 0.5 0.5的加, 不过后来想了下可能用的比较少, 所以就先删了
            for (int i = 0; i < finalParsedContent.Length; i++)
            {
                TMP_CharacterInfo charInfo = TextInfo.characterInfo[i];
                // 跳过空白符
                if (!charInfo.isVisible)
                    continue;

                // 不知道为什么分开 update 颜色会抽搐, 感觉跟 tmp 内部更新机制有关, 类似改了颜色但实际上没改导致访问到了错误的颜色之类的
                hyperLink?.ApplyLinkColors();
                // 简化逻辑：只根据 showCharNum 判断
                byte alpha = i < showCharNum ? (byte)255 : (byte)0;
                SetAlpha(charInfo.vertexIndex, alpha);
            }

            tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        private void SetAlpha(int idx, byte alpha)
        {
            Color32 color = TextInfo.meshInfo[0].colors32[idx + 0];
            color.a = alpha;

            TextInfo.meshInfo[0].colors32[idx + 0] = color;
            TextInfo.meshInfo[0].colors32[idx + 1] = color;
            TextInfo.meshInfo[0].colors32[idx + 2] = color;
            TextInfo.meshInfo[0].colors32[idx + 3] = color;
        }


        private void ParseString()
        {
            // 流程大概是这样的
            // 首先赋值原生文本 RawContent, 也就是在配置文件里的最纯的文本
            // 然后获得 tmp 解析过的文本, 在此基础上我们 parse 自己的标签来获知我们的标签应该作用在位于哪些 index 的字符上
            // 然后把原生文本去掉我们的标签后赋值给 tmp 即可

            SetText(RawContent);
            tmpParsedContent = tmpText.GetParsedText();

            var s = new StringBuilder(tmpParsedContent);
            var effects = new List<TextEffectBase>();

            // 解析所有标签
            foreach (var textEffect in textEffects)
                effects.AddRange(textEffect.ParseAndCoverTag(s));

            // 构建索引映射（合并两个循环）
            var sIndexToRealIndex = new Dictionary<int, int>();
            var finalContent = new StringBuilder(s.Length);

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != TextEffectBase.Placeholder)
                {
                    sIndexToRealIndex[i] = finalContent.Length;
                    finalContent.Append(s[i]);
                }
            }

            finalParsedContent = finalContent.ToString();

            // 调整范围并过滤（反向遍历以便安全删除）
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                if (!TryAdjustRange(effects[i], sIndexToRealIndex))
                {
                    effects.RemoveAt(i);
                }
            }

            // 分类处理
            foreach (var info in effects)
            {
                if (info is EventEffect)
                    charPosToEventInfo[info.start].Merge(info.args);
                else
                    currentEffects.Add(info);
            }

            // 生成最终文本
            var s2 = new StringBuilder(RawContent);
            foreach (var textEffect in textEffects)
                textEffect.ParseAndCoverTag(s2);

            SetText(s2.ToString().Replace(TextEffectBase.Placeholder.ToString(), ""));

            hyperLink?.ResetLink();
        }

        private bool TryAdjustRange(TextEffectBase effect, Dictionary<int, int> indexMap)
        {
            // 调整起始位置
            while (effect.length > 0 && !indexMap.ContainsKey(effect.start))
            {
                effect.start++;
                effect.length--;
            }

            // 调整结束位置
            while (effect.length > 0 && !indexMap.ContainsKey(effect.start + effect.length - 1))
            {
                effect.length--;
            }

            if (effect.length == 0)
                return false;

            // 计算真实索引
            effect.length = indexMap[effect.start + effect.length - 1] - indexMap[effect.start] + 1;
            effect.start = indexMap[effect.start];
            return true;
        }
    }
}