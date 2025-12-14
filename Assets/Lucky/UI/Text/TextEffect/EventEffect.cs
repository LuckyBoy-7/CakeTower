using System.Text.RegularExpressions;

namespace Lucky.UI.Text.TextEffect
{
    public abstract class EventEffect : TextEffectBase
    {
        public override int ExceptGroup => -1;

        public override TextEffectBase HandleMatchedGroup(Match match)
        {
            TextEffectBase effect = CreateInstance();
            effect.start = match.Groups[0].Index + match.Groups[0].Length;
            effect.length = 1;

            string tag = match.Groups[0].Value;
            string tagName = tag.Substring(1, tag.IndexOf("=") - 1);
            effect.args = new() { {tagName, match.Groups[1].Value } };
            return effect;
        }
    }
}