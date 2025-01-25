using Mirror;
using Mirror.Examples.Tanks;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// Spawns bubbles that float upward when the player presses 'F'.
/// Attach this script to a GameObject in the scene and assign a bubble prefab.
/// </summary>
public class BubbleSpawner : NetworkBehaviour
{
	public GameObject bubblePrefab;
	public Color bubbleColor;

	GameObject _bubble;

	public void SpawnBubble()
	{
		if (isLocalPlayer)
		{
			CmdSpawnBubble();
		}
	}

	[Command]
	public void CmdSpawnBubble()
	{
		if (_bubble != null)
		{
			_bubble.GetComponent<BubbleBehaviour>().DestroyBubble();
		}
		// offset by gameobject's facing direction
		var collider = GetComponent<BoxCollider2D>();
		float bubbleRadius = bubblePrefab.GetComponent<CircleCollider2D>().radius * bubblePrefab.transform.localScale.x;
		float xOffset = MathF.Sign(transform.localScale.x) * (0.5f * collider.bounds.size.x + bubbleRadius + 0.02f);
		float yOffset = collider.offset.y * transform.localScale.y - 0.5f * collider.bounds.size.y + bubbleRadius + 0.02f;

		Vector3 spawnPos = new Vector3(transform.position.x + xOffset,
									   transform.position.y + yOffset,
									   transform.position.z);

		// Instantiate the bubble
		GameObject newBubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
		newBubble.GetComponent<SpriteRenderer>().color = bubbleColor;

		// Add the BubbleBehaviour script to control the bubble's movement
		BubbleBehaviour bubbleBehavior = newBubble.GetComponent<BubbleBehaviour>();
		NetworkServer.Spawn(newBubble);
		bubbleBehavior.driftDirection = xOffset > 0 ? 1 : -1;
		_bubble = newBubble;
	}
}

