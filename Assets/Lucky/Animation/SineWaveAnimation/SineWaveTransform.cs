using UnityEngine;

namespace Lucky.Animation.SineWaveAnimation
{
    public abstract class SineWaveTransform : MonoBehaviour
    {
        public Vector3 magnitude; // 暴露给编辑器调整的
        public float speed = 0;
        public float timeOffset = 0; // sin初相

        private Vector3 originalMagnitude; // 初始值，相当于基准值

        private void Awake()
        {
            originalMagnitude = GetMagnitude();
        }

        public void Update()
        {
            float sine = Mathf.Sin(Time.time * speed + timeOffset) ;
            float x = originalMagnitude.x + (sine * magnitude.x);
            float y = originalMagnitude.y + (sine * magnitude.y);
            float z = originalMagnitude.z + (sine * magnitude.z);
            ApplyTransformation(new Vector3(x, y, z));
        }

        protected abstract Vector3 GetMagnitude();
        protected abstract void ApplyTransformation(Vector3 value);
    }
}