using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// using Sirenix.OdinInspector;

namespace Lucky.UI.Text.TextEffect
{
    public abstract class TextEffectBase
    {
        public TMP_Text tmpText;
        protected TMP_TextInfo TextInfo => tmpText.textInfo;

        public const char Placeholder = '\0';

        public int start;
        public int length;
        public Dictionary<string, string> args;


        protected void SetVerticesOffset(int idx, Vector3 offset)
        {
            TextInfo.meshInfo[0].vertices[idx + 0] += offset;
            TextInfo.meshInfo[0].vertices[idx + 1] += offset;
            TextInfo.meshInfo[0].vertices[idx + 2] += offset;
            TextInfo.meshInfo[0].vertices[idx + 3] += offset;
        }

        protected void SetColor(int idx, Color32 color)
        {
            TextInfo.meshInfo[0].colors32[idx + 0] = color;
            TextInfo.meshInfo[0].colors32[idx + 1] = color;
            TextInfo.meshInfo[0].colors32[idx + 2] = color;
            TextInfo.meshInfo[0].colors32[idx + 3] = color;
        }

        public abstract string GetPattern();
        public virtual int ExceptGroup => 2;
        public abstract TextEffectBase CreateInstance();

        public virtual TextEffectBase HandleMatchedGroup(Match match)
        {
            TextEffectBase effect = CreateInstance();
            effect.start = match.Groups[2].Index;
            effect.length = match.Groups[2].Length;
            effect.args = ParseSelector(match.Groups[1].Value);
            return effect;
        }

        public List<TextEffectBase> ParseAndCoverTag(StringBuilder s)
        {
            List<TextEffectBase> res = new();
            // <selector prop1=val1, prop2 = val2> content </selector>
            string pattern = GetPattern();
            foreach (Match match in ParseTag(s, pattern, new ParseTagOptions { ExceptGroup = ExceptGroup }))
            {
                res.Add(HandleMatchedGroup(match));
            }

            return res;
        }


        public virtual void TakeEffect()
        {
        }


        public class ParseTagOptions
        {
            public bool Replace { get; set; } = true;
            public char Placeholder { get; set; } = TextEffectBase.Placeholder;
            public int ExceptGroup { get; set; } = -1; // -1 表示替换整体
        }

        /// 解析字符串获得所有的match，并选择性替换原文本
        public static List<Match> ParseTag(StringBuilder s, string pattern, ParseTagOptions options = null)
        {
            options ??= new ParseTagOptions();
            List<Match> res = new();

            foreach (Match match in Regex.Matches(s.ToString(), pattern))
            {
                if (options.Replace)
                {
                    ReplaceRange(s, options.Placeholder, match.Groups[0], options.ExceptGroup != -1 ? match.Groups[options.ExceptGroup] : null);
                }

                res.Add(match);
            }

            return res;
        }

        private static void ReplaceRange(StringBuilder s, char placeholder, Group replaceGroup, Group exceptGroup = null)
        {
            for (int i = replaceGroup.Index; i < replaceGroup.Index + replaceGroup.Length; i++)
            {
                if (exceptGroup == null || i < exceptGroup.Index || i >= exceptGroup.Index + exceptGroup.Length)
                    s[i] = placeholder;
            }
        }

        /// 解析形如"prop1=val1; prop2=val2"这样的字符串，转化为{"prop1": "val1", "prop2": "val2"}
        public static Dictionary<string, string> ParseSelector(string s)
        {
            Dictionary<string, string> res = new();
            foreach (string pair in s.Split(";"))
            {
                string p = pair.Trim();
                if (string.IsNullOrEmpty(p))
                    continue;
                var pairSplit = p.Split("=");
                if (pairSplit.Length != 2) // 一般是防止debug的时候在打字过程中只有一个元素
                    continue;
                string property = pairSplit[0].Trim();
                string value = pairSplit[1].Trim();
                if (string.IsNullOrEmpty(property)) // 都是防止debug的时候报错
                    continue;
                if (string.IsNullOrEmpty(value))
                    continue;
                res[property] = value;
            }

            return res;
        }
    }
}