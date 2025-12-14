using UnityEngine;

namespace Lucky.Animation.Line
{
    public class LineConnector : MonoBehaviour
    {
        public Transform from;
        public Transform to;

        public LineRenderer line;

        public bool useDottedLine;

        public static Material defaultMaterial;
        public static Material dottedLineMaterial;
        public float lineFlowSpeed = 1f;
        public float dotCountPerUnit = 0.015f;

        private void Awake()
        {
            if (defaultMaterial == null)
                defaultMaterial = new Material(Shader.Find("Sprites/Default"));
            if (dottedLineMaterial == null)
                dottedLineMaterial = new Material(Resources.Load<Material>("Materials/Line/DottedLine"));
            
            
            line = new GameObject("Line").AddComponent<LineRenderer>();
            line.transform.localPosition = Vector3.zero;
            line.useWorldSpace = true;
            line.startColor = line.endColor = UnityEngine.Color.white;
            line.startWidth = line.endWidth = 20; 
            line.enabled = false;
        }

        private void Start()
        {
            line.material = useDottedLine ? dottedLineMaterial : defaultMaterial;
        }

        private void Update()
        {
            if (from == null || to == null)
            {
                line.enabled = false;
                return;
            }

            line.enabled = true;
            line.positionCount = 2;
            line.SetPosition(0, from.position);
            line.SetPosition(1, to.position);

            if (useDottedLine)
            {
                Vector2 offset = line.material.mainTextureOffset;
                offset.x -= lineFlowSpeed * Time.deltaTime;
                line.material.mainTextureOffset = offset;
                
                float distance = Vector3.Distance(from.position, to.position);
                float tilingFactor = distance * dotCountPerUnit;
                line.material.mainTextureScale = new Vector2(tilingFactor, 1);
            }
        }
    }
}