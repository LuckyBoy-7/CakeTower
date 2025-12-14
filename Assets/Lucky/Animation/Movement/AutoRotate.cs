using UnityEngine;

namespace Lucky.Animation.Movement
{
    /// <summary>
    /// 自动旋转
    /// </summary>
    public class AutoRotate : MonoBehaviour
    {
        public Vector3 rotationSpeed = default;
        public bool local = default;

        public void FixedUpdate()
        {
            Vector3 rotateAmount = rotationSpeed * Time.deltaTime;
            if (local)
            {
                transform.Rotate(rotateAmount.x, rotateAmount.y, rotateAmount.z, Space.Self);
            }
            else
            {
                transform.Rotate(rotateAmount.x, rotateAmount.y, rotateAmount.z, Space.World);
            }
        }
    }
}