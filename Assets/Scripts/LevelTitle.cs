using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LevelTitle : MonoBehaviour
{
	public float showTime = 1;
	public float fadeTime = 1;
    Image image;
	private void Awake()
	{
		image = GetComponent<Image>();
	}

	private void OnEnable()
	{
		Color color = image.color;
		color.a = 0;
		image.color = color;
		image.DOFade(1, showTime).SetEase(Ease.OutCubic).OnComplete(() => { image.DOFade(0, fadeTime).SetEase(Ease.InCubic); });
	}
}
