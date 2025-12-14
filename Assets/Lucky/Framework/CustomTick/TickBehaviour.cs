using System;
using Lucky.Extensions;
using Lucky.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lucky.Framework.CustomTick
{
    /// <summary>
    /// 本来想写一个 manager 的, 但是后来突然意识到同类型脚本 unity 应该会一起执行而且帮我控制顺序, 所以理论上我只需要去管理 components 就行了
    /// </summary>
    public abstract class TickBehaviour : MonoBehaviour
    {
        public TickComponentList components { get; private set; }

        [FormerlySerializedAs("_collider")] [SerializeField]
        private Collider _unityCollider;

        public Collider UnityCollider
        {
            get
            {
                if (_unityCollider != null)
                    return _unityCollider;
                _unityCollider = GetComponent<Collider>();
                return _unityCollider;
            }
            set => _unityCollider = value;
        }


        protected virtual void Awake()
        {
            components = new TickComponentList(this);
            if (Tracker.Contains(this))
            {
                Tracker.Add(this);
            }
        }


        protected virtual void FixedUpdate()
        {
            if (!Engine.Instance.FreezeFixedUpdate)
                Tick();
        }

        protected virtual void Tick()
        {
            components.Tick();
        }


        public void Add(TickComponent tickComponent)
        {
            components.Add(tickComponent);
        }

        public void Remove(TickComponent tickComponent)
        {
            components.Remove(tickComponent);
        }

        protected virtual void OnDestroy()
        {
            if (Tracker.Contains(this))
            {
                Tracker.Remove(this);
            }
        }

        #region CollideCheck

        public bool CollideCheck(TickBehaviour other)
        {
            return ColliderUtils.CollideCheck(UnityCollider, other.UnityCollider);
        }

        public bool CollideCheck(TickBehaviour other, Vector3 offset)
        {
            Vector3 origPosition = transform.position;
            transform.position += offset;
            bool res = ColliderUtils.CollideCheck(UnityCollider, other.UnityCollider);
            transform.position = origPosition;
            return res;
        }

        #endregion
    }
}