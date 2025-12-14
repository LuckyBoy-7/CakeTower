using UnityEngine;

namespace Lucky.Animation.SineWaveAnimation
{
    public class SineWaveScale : SineWaveTransform
    {
        protected override Vector3 GetMagnitude()
        {
            return transform.localScale;
        }

        protected override void ApplyTransformation(Vector3 value)
        {
            transform.localScale = value;
        }
    }
}