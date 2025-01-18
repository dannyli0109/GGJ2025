using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelGroup : MonoBehaviour
{
	public GameObject levelBtnPrefab;

	public Sprite[] levelSprites;

	private void Awake()
	{
		int sceneCount = SceneManager.sceneCountInBuildSettings;
		for (int i = 1; i < sceneCount; i++)
		{
			Debug.Log(i);
			var obj = Instantiate(levelBtnPrefab, transform);
			//obj.GetComponentInChildren<Text>().text = i.ToString();
			obj.GetComponent<Image>().sprite = levelSprites[i-1];
			var btn = obj.GetComponent<Button>();
			int index = i;
			btn.onClick.AddListener(() => { GameManager.Instance.SwitchScene(index); });
		}
	}
}
