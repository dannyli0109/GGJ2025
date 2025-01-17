using UnityEngine;

namespace UnityUtils
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        public bool persistent = true;

        public bool autoUnparentOnAwake = true;

        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();
                    if (instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        instance = go.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            if (!Application.isPlaying) return;

            if (autoUnparentOnAwake)
            {
                transform.SetParent(null);
            }

            if (instance == null)
            {
                instance = this as T;
                if (persistent)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                if (instance != this)
                {
                    DestroyImmediate(gameObject);
                }
            }
        }
    }
}