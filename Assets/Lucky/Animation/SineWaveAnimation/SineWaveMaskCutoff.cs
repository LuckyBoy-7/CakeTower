using Lucky.Utilities;
using UnityEngine;


namespace Lucky.Animation.SineWaveAnimation
{
    public class SineWaveMaskCutoff : MonoBehaviour
    {
        [SerializeField] private SpriteMask mask = default;

        [SerializeField] private float speed = default;

        [SerializeField] private float minValue = default;

        [SerializeField] private float maxValue = default;

        float offset = default;

        private void Awake()
        {
            offset = RandomUtils.NextRadians(); // 在一个周期内随便选个点
        }

        public void FixedUpdate()
        {
            float sine = (Mathf.Sin(Time.time * speed + offset) * 0.5f) + 0.5f; // [0, 1]
            mask.alphaCutoff = Mathf.Lerp(minValue, maxValue, sine);
        }
    }
}
