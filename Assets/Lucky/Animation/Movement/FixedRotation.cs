using Sirenix.OdinInspector;
using UnityEngine;

namespace Lucky.Animation.Movement
{
    /// <summary>
    /// 固定旋转，定死了
    /// </summary>
    public class FixedRotation : MonoBehaviour
    {
        public bool fixX;
        [ShowIf("fixX")] public float xRotation;

        public bool fixY;
        [ShowIf("fixY")] public float yRotation;

        public bool fixZ;
        [ShowIf("fixZ")] public float zRotation;

        public void FixedUpdate()
        {
            float x = fixX ? xRotation : transform.eulerAngles.x;
            float y = fixY ? yRotation : transform.eulerAngles.y;
            float z = fixZ ? zRotation : transform.eulerAngles.z;

            transform.eulerAngles = new Vector3(x, y, z);
        }
    }
}