using UnityEngine;

namespace Lucky.Framework
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    _instance?.SingletonAwake();
                }

                return _instance;
            }
        }


        private void Awake()
        {
            if (_instance == this)
                return;

            if (_instance == null)
            {
                _instance = (T)this;
                SingletonAwake();
            }
            else
                Destroy(gameObject);
        }

        protected virtual void SingletonAwake()
        {
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}