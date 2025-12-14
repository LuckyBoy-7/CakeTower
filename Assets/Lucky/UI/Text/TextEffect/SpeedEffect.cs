namespace Lucky.UI.Text.TextEffect
{
    public class SpeedEffect : EventEffect
    {
        public override string GetPattern() => @"<speed=(\d+\.?\d*)/>";
        public override TextEffectBase CreateInstance() => new SpeedEffect { tmpText = tmpText };
    }
}