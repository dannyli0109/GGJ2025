using Cinemachine;
using Mirror;
using Mirror.BouncyCastle.Asn1.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class GameManager : SingletonMono<GameManager>
{
	[Serializable]
	public struct Level
	{
		[Scene] public string scene;
		public AudioClip bgm;
	}

	public List<Level> levelList;
	[Scene] public string mainScene;
	public AudioClip mainBgm;
	MyNetworkRoomManager networkManager;
	AudioSource bgmSource;
	[HideInInspector] public List<Transform> targets = new List<Transform>();

	protected override void Init()
	{
		networkManager = NetworkManager.singleton as MyNetworkRoomManager;
		bgmSource = GetComponent<AudioSource>();
		SceneManager.sceneLoaded += OnSceneLoaded;
		bgmSource.clip = mainBgm;
		bgmSource.Play();
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (mode == LoadSceneMode.Single)
		{
			foreach (var level in levelList)
			{
				if (level.scene == scene.path)
				{
					bgmSource.clip = level.bgm;
					bgmSource.Play();
				}
			}
		}
	}

	public void AddTarget(Transform transform)
	{
		var targetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
		targetGroup?.AddMember(transform, 1, 0);
		targets.Add(transform);
	}

	public void RemoveTarget(Transform transform)
	{
		var targetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
		targetGroup?.RemoveMember(transform);
		targets.Remove(transform);
	}

	public void PlayAudioOnce(AudioClip clip, Vector2 pos)
	{
		float time = clip.length;
		var obj = new GameObject(clip.name);
		obj.transform.position = pos;
		var audioSource = obj.AddComponent<AudioSource>();
		audioSource.loop = false;
		audioSource.clip = clip;
		audioSource.Play();
		Destroy(obj, time);
	}

	public int GetCurLevelId()
	{
		string path = SceneManager.GetActiveScene().path;
		for (int i = 0; i < levelList.Count; i++)
		{
			Level level = levelList[i];
			if (level.scene == path)
			{
				return i;
			}
		}
		return -1;
	}

	public void SwitchMainScene()
	{
		if (NetworkServer.active)
		{
			SwitchScene(mainScene);
			bgmSource.clip = mainBgm;
			bgmSource.Play();
		}
	}

	public void RestartLevel()
	{
		SwitchScene(SceneManager.GetActiveScene().path);
	}

	public void SwitchNextLevel()
	{
		if (NetworkServer.active)
		{
			int id = GetCurLevelId();
			if (id >= 0)
			{
				SwitchLevel(id + 1);
			}
		}
	}

	public void SwitchLevel(int id)
	{
		if (NetworkServer.active)
		{
			id = id % levelList.Count;
			string sceneName = levelList[id].scene;
			SwitchScene(sceneName);
		}
	}

	public void SwitchScene(string name)
	{
		if (NetworkServer.active)
		{
			targets.Clear();
			networkManager.ChangeScene(name);
		}
	}

	void Update()
	{
		// when r is pressed, restart the current scene
		if (Input.GetKeyDown(KeyCode.R))
		{
			Restart();
		}
		if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
		{
			SwitchLevel(0);
		}
	}

	public void _Restart()
	{
		// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

		SwitchLevel(SceneManager.GetActiveScene().buildIndex);
	}

	public void Restart()
	{
		// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		SwitchScene(SceneManager.GetActiveScene().name);
	}
}
