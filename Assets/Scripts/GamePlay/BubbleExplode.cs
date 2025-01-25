using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleExplode : MonoBehaviour
{

	public void DestroySelf()
	{
		// Play sound

		// Play particle effect
		// Destroy bubble
		if (Application.isPlaying)
		{
			Destroy(gameObject);
		}
	}
}
