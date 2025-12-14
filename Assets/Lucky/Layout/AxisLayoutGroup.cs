using UnityEngine;

namespace Lucky.Layout
{
    public class AxisLayoutGroup : MonoBehaviour
    {
        public bool controlX;
        public bool controlY;
        public float spaceX;
        public float spaceY;

        private void Update()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                float nx = child.transform.position.x;
                if (controlX)
                    nx = transform.position.x + spaceX * i;

                float ny = child.transform.position.y;
                if (controlY)
                    ny = transform.position.y + spaceY * i;

                child.position = new Vector3(nx, ny, child.position.z);
            }
        }
    }
}