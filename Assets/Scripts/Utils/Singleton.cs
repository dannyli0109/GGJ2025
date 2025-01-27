using UnityEngine;

namespace Utils
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
					return new GameObject(nameof(T)).AddComponent<T>();
				}
				else
				{
					return instance;
				}
			}
		}

		protected virtual void Awake()
		{
			if (!Application.isPlaying) return;

			if (autoUnparentOnAwake)
			{
				transform.SetParent(null);
			}

			if (instance == null)
			{
				instance = this as T;
				Init();
				if (persistent)
				{
					DontDestroyOnLoad(gameObject);
				}
			}
			else
			{
				if (Instance != this)
				{
					DestroyImmediate(gameObject);
				}
			}
		}

		protected virtual void Init()
		{
		}
	}
}