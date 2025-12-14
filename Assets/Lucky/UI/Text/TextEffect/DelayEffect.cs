namespace Lucky.UI.Text.TextEffect
{
    public class DelayEffect : EventEffect
    {
        public override string GetPattern() => @"<delay=(\d+\.?\d*)/>";
        public override TextEffectBase CreateInstance() => new DelayEffect { tmpText = tmpText };
    }
}