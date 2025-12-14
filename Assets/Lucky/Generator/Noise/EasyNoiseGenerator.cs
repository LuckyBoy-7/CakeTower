using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Generator.Noise
{
    public class EasyNoiseGenerator : NoiseGenerator
    {
        protected override string FileName => "EasyNoise.png";

        protected override Color GetPixelColor(int x, int y) => Color.white * RandomUtils.NextFloat(1);
    }
}