using UnityEngine;
using System.Collections.Generic;
using Utils;

[CreateAssetMenu(menuName = "Data/Property/BubbleSpawnerData", fileName = "BubbleSpawnerData")]
public class BubbleSpawnerData : ScriptableObject
{
	[Header("Bubble Prefab")]
	[Tooltip("Drag and drop your bubble explode effect prefab here.")]
	public GameObject effectBubbleExplodePrefab;
	[Header("Movement Settings")]
	[Tooltip("Upward speed for bubbles.")]
	public float riseSpeed = 0.1f;
	[HideInInspector]
	public float airDragFactor;
	[HideInInspector]
	public float buoyancy;
	public float pushHorizontalAngle = 90;
	[Tooltip("The range of horizontal drifting speed.")]
	public float horizontalDrift = 0.3f;
	[Tooltip("The distance to move opposite direction")]
	public float bouncebackDistance = 1;
	[Tooltip("Push horizontal speed")]
	public float pushSpeed = 4;
	[Tooltip("Max horizontal distance")]
	public float maxHorizontalDistance = 3;
	[Tooltip("Time to destroy the bubble")]
	public float destroyTime = 10f;
	[Tooltip("bubble pop sound")]
	public List<AudioClip> popClips;
	[Tooltip("blow bubble sound")]
	public List<AudioClip> blowClips;
	[Tooltip("bounce")]
	public List<AudioClip> bounceClips;

	private void OnValidate()
	{
		airDragFactor = Math2D.CalAirDragFactor(maxHorizontalDistance, pushSpeed);
		buoyancy = Math2D.CalAirDrag(riseSpeed, airDragFactor);
		Debug.Log("OnValidate called! Value is: " + airDragFactor);
	}
}
