using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

public class GameManager : SingletonMono<GameManager>
{
	AudioSource bgmSource;
	public List<AudioClip> bgmClips;

	protected override void Awake()
	{
		base.Awake();
		bgmSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		SwitchBgm(0);
	}

	public void SwitchNextScene()
	{
		int id = SceneManager.GetActiveScene().buildIndex;
		SwitchScene(id + 1);
	}

	public void SwitchScene(int id)
	{
		SceneManager.LoadScene(id);
	}

	public void SwitchBgm(int id)
	{
		if (id < bgmClips.Count)
		{
			bgmSource.clip = bgmClips[id];
			bgmSource.Play();
		}
	}
}
