using Lucky.Extensions;
using UnityEngine;

namespace Lucky.Shaders.Misc.WaterMirror.Camera
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class WaterMirror : MonoBehaviour
    {
        private SpriteRenderer sr;
        private UnityEngine.Camera waterCamera;
        private RenderTexture renderTexture;
        public Material material;
        public int precise = 100;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            waterCamera = new GameObject("WaterMirrorCamera").AddComponent<UnityEngine.Camera>();
            waterCamera.orthographic = true;
            // 先把湖面的比例搞对了, 这里保持高度, 缩放宽度
            float aspect = waterCamera.aspect; // w / h
            float height = transform.localScale.y;
            float width = aspect * height;
            transform.localScale = transform.localScale.WithX(width);

            // 调整相机大小
            waterCamera.orthographicSize = height / 2;

            // 把相机移到上面
            waterCamera.transform.position = transform.position + Vector3.up * height;

            renderTexture = new RenderTexture((int)width * precise, (int)height * precise, 0);
            renderTexture.filterMode = FilterMode.Point;
            renderTexture.wrapMode = TextureWrapMode.Repeat;
            waterCamera.targetTexture = renderTexture;

            // 可见
            waterCamera.transform.position = waterCamera.transform.position.WithZ(-10);
            waterCamera.clearFlags = CameraClearFlags.Depth;
        }

        private void Update()
        {
            material.SetTexture("_ViewTex", renderTexture);
        }
    }
}