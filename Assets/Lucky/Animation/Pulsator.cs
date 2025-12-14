using UnityEngine;

namespace Lucky.Animation
{
    public class Pulsator : MonoBehaviour
    {
        public float amount = 0.06f;
        public float speed = 20f;

        private float k = -1f;

        private Vector3 origSize;

        private void Awake()
        {
            origSize = transform.localScale;
        }

        public void Update()
        {
            if (k >= 0f)
            {
                k = Mathf.MoveTowards(k, 1f, Time.deltaTime * speed);
                var stepped = Mathf.SmoothStep(0f, 1f, k);
                // [1, 2]
                var size = Mathf.Sin(Mathf.PI * stepped) * amount + 1f;
                transform.localScale = size * origSize;
            }
        }

        public void Pulsate()
        {
            k = 0f;
        }
    }
}