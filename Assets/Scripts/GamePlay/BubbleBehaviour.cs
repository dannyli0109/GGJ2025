using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controls the movement and destruction of individual bubble objects.
/// </summary>
public class BubbleBehaviour : NetworkBehaviour
{
	public BubbleSpawnerData data;
	[HideInInspector]
	public float driftDirection;

	float _distance = 0;
	bool _shouldMove = true;
	float _horizontalDistance = 0;
	bool _hasBeenPushed = false;        // Once true, bubble moves in pushDirection only
	Vector2 _pushDirection = Vector2.zero;  // The direction (unit vector) of the push
	float pushPower = 50;

	private void Start()
	{
		PlayBlowAudio();
	}

	public override void OnStartServer()
	{
		Invoke(nameof(DestroyBubble), data.destroyTime);
	}

	[Command(requiresAuthority = false)]
	public void CmdDestroyBubble()
	{
		DestroyBubble();
	}

	[Server]
	public void DestroyBubble()
	{
		NetworkServer.Destroy(gameObject);
	}

	void PlayPopAudio()
	{
		var clip = data.popClips[UnityEngine.Random.Range(0, data.popClips.Count)];
		GameManager.Instance.PlayAudioOnce(clip, transform.position);
	}

	void PlayBlowAudio()
	{
		var clip = data.blowClips[UnityEngine.Random.Range(0, data.blowClips.Count)];
		GameManager.Instance.PlayAudioOnce(clip, transform.position);
	}

	void PlayBounceAudio()
	{
		var clip = data.bounceClips[UnityEngine.Random.Range(0, data.bounceClips.Count)];
		GameManager.Instance.PlayAudioOnce(clip, transform.position);
	}

	[ServerCallback]
	private void Update()
	{
		if (!_shouldMove) return;
		if (_hasBeenPushed)
		{
			// transform.Translate(_pushDirection * pushSpeed * Time.deltaTime, Space.World);
			// slow down horizontal speed as it approaches max distance
			// it will be fast -> slow
			if (_horizontalDistance > data.maxHorizontalDistance * 0.99f)
			{
				_horizontalDistance = data.maxHorizontalDistance;
				_hasBeenPushed = false;
			}

			if (_hasBeenPushed)
			{
				// faster at the beginning
				float percent = _horizontalDistance / data.maxHorizontalDistance;
				// if (percent > 0.95) percent = 1; and snap to max distance
				float speed = data.pushHorizontalSpeed * (1 - percent * percent * percent * percent);
				Vector3 movement = new Vector3(
					_pushDirection.x * speed * Time.deltaTime,
					_pushDirection.y * data.pushVerticalSpeed * Time.deltaTime,
					0f
				);

				if (_horizontalDistance + Mathf.Abs(movement.x) > data.maxHorizontalDistance)
				{
					movement.x = data.maxHorizontalDistance - _horizontalDistance;
				}

				transform.Translate(movement);
				_horizontalDistance += Mathf.Abs(movement.x);
			}

			// If you still want the bubble to be destroyed off-screen:
		}

		// -----------------------------------------------------------
		// Existing drifting logic (only used if NOT pushed yet)
		// -----------------------------------------------------------
		if (_shouldMove)
		{
			// Move the bubble upward + drift
			Vector3 movement = new Vector3(
				driftDirection * data.horizontalDrift * Time.deltaTime,
				data.bubbleSpeed * Time.deltaTime,
				0f
			);

			// Track horizontal distance for bounceback
			_distance += movement.x;
			if (Mathf.Abs(_distance) > data.bouncebackDistance)
			{
				driftDirection *= -1;
				_distance = 0;
			}

			float percent = _horizontalDistance / data.maxHorizontalDistance;
			if (percent < 1 && _hasBeenPushed)
			{
				movement.x = 0;
			}

			// Apply the movement
			transform.Translate(movement);
		}
	}

	/// <summary>
	/// When the player first lands on the bubble, make the player a child of the bubble.
	/// This lets the player ride the bubble.
	/// </summary>
	[ServerCallback]
	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Check if it's the player colliding
		if (collision.gameObject.CompareTag("Player"))
		{
			// only do the logic when the player is on top of the bubble
			if (collision.contacts[0].normal.y > 0) return;
			// Make the player a child of the bubble
			// collision.transform.SetParent(transform, true);
			_shouldMove = false;    // Stop the bubble from moving
		}
	}

	[ServerCallback]
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			// Reset the player's parent to null
			// collision.transform.SetParent(null, true);
			_shouldMove = true;    // Allow the bubble to move again
		}
	}

	[ServerCallback]
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Ground"))
		{
			DestroyBubble();
		}

		if (other.CompareTag("Player"))
		{
			// PlayBounceAudio();
		}
	}

	[ServerCallback]
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			//PlayBounceAudio();
		}
	}

	[ServerCallback]
	private void OnTriggerStay2D(Collider2D other)
	{
		if (!other.CompareTag("Player")) return;


		// If the bubble is already pushed, don't re-push it

		// Vector from player's position to bubble's position
		Vector2 bubblePos = transform.position;
		Vector2 playerPos = other.transform.position;
		Vector2 direction = (bubblePos - playerPos);

		// Only push if the player is actually below or to the side (y > 0 => bubble is above the player)
		if (direction.y <= 0)
		{
			// If you want symmetrical logic from above, remove or invert this check.
			return;
		}

		// Compute how far from straight-up this direction is
		float angle = Vector2.SignedAngle(Vector2.up, direction);
		float absAngle = Mathf.Abs(angle);

		// Decide final push direction based on angle thresholds
		Vector2 finalDir;

		if (other.GetComponent<MovementController>().isOnGround)
		{
			// only check which side the player is on
			finalDir = (playerPos.x > bubblePos.x) ? Vector2.left : Vector2.right;
		}
		else
		{
			// 0бу - 22.5бу => straight up
			if (absAngle < 22.5f)
			{
				finalDir = Vector2.up;
			}
			// 22.5бу - 67.5бу => diagonal (б└45бу up)
			else if (absAngle < 67.5f)
			{
				// If angle > 0 => direction is "left" from the bubble's perspective, so push up-left
				finalDir = Vector2.up;
			}
			else
			{
				return;
			}
		}
		// Record that we got pushed in this final direction
		_pushDirection = finalDir;
		_hasBeenPushed = true;
		_shouldMove = true;  // (Optional) Stop the drift if you want
		_horizontalDistance = 0;
	}

	/// <summary>
	/// Detach the player (or any children) before the bubble is destroyed.
	/// </summary>
	private void OnDestroy()
	{
		if (Application.isPlaying)
		{
			var effectObj = Instantiate(data.effectBubbleExplodePrefab, transform.position, Quaternion.identity);
			effectObj.transform.localScale = transform.localScale;
			effectObj.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
			PlayPopAudio();
		}
		// For safety, ensure no child remains parented
		// (especially the player, if still attached).
		//for (int i = transform.childCount - 1; i >= 0; i--)
		//{
		//	Transform child = transform.GetChild(i);

		//	// If you're specifically looking for "Player", do:
		//	if (child.CompareTag("Player"))
		//		child.SetParent(null, true);
		//}
	}
}
