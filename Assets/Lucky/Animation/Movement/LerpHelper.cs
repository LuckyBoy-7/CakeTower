using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Lucky.Animation.Movement
{
    public class LerpHelper : MonoBehaviour
    {
        [SerializeField] private Vector3 _targetLocalPos;

        public Vector3 TargetLocalPos
        {
            get => _targetLocalPos;
            set
            {
                started = true;
                _targetLocalPos = value;
            }
        }

        public float k = 0.05f;
        [SerializeField] private bool started;
        [SerializeField] private bool inited;

        private Func<Vector3> LocalPositionGetter;
        private Action<Vector3> LocalPositionSetter;

        private void Start()
        {
            Assert.IsNotNull(LocalPositionGetter, "getter 未赋值");
            Assert.IsNotNull(LocalPositionSetter, "setter 未赋值");
        }

        public void Init(Func<Vector3> getter, Action<Vector3> setter)
        {
            Assert.IsNotNull(getter, "getter 不能为 null");
            Assert.IsNotNull(setter, "setter 不能为 null");
            LocalPositionGetter = getter;
            LocalPositionSetter = setter;
        }


        private void FixedUpdate()
        {
            if (started)
            {
                LocalPositionSetter(Vector3.Lerp(LocalPositionGetter(), TargetLocalPos, k));
            }
        }
    }
}