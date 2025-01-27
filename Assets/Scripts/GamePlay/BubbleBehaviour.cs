using Mirror;
using Mirror.BouncyCastle.Tls;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

/// <summary>
/// Controls the movement and destruction of individual bubble objects.
/// </summary>
public class BubbleBehaviour : NetworkBehaviour
{
	public BubbleSpawnerData data;
	Rigidbody2D rig;

	private void Awake()
	{
		rig = GetComponent<Rigidbody2D>();
		rig.gravityScale = data.buoyancy / Physics2D.gravity.y;
	}

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

	//[ServerCallback]
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			Vector2 v = rig.velocity;
			v.x = -data.pushSpeed;
			rig.velocity = v;
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			Vector2 v = rig.velocity;
			v.x = data.pushSpeed;
			rig.velocity = v;
		}
	}

	//[ServerCallback]
	private void FixedUpdate()
	{
		AddAirDragForce();
		Debug.Log(rig.velocity);
	}

	void AddAirDragForce()
	{
		Vector2 v = rig.velocity;
		Vector2 force = -v.normalized * Math2D.CalAirDrag(v.magnitude, data.airDragFactor) * rig.mass;
		rig.AddForce(force);
	}

	void Push(Vector2 dir)
	{
		rig.velocity = dir.normalized * data.pushSpeed;
	}

	IEnumerator Swing(float distance, float time, bool beginRight = true)
	{
		float halfTime = time / 2;
		float a = distance / (halfTime * halfTime);
		a = beginRight ? a : -a;
		Vector2 velocity = rig.velocity;
		velocity.x = a * halfTime;
		rig.velocity = velocity;
		float t = 0;
		while (true)
		{
			yield return null;
			t += Time.deltaTime;
			velocity = rig.velocity;
			if (t < halfTime)
			{
				velocity.x -= a * Time.deltaTime;
			}
			else if (t < time)
			{
				velocity.x += a * Time.deltaTime;
			}
			else
			{
				t = 0;
			}
			rig.velocity = velocity;
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
		}
	}

	[ServerCallback]
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			// Reset the player's parent to null
			// collision.transform.SetParent(null, true);
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
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Ground"))
		{
			DestroyBubble();
		}
		if (!other.CompareTag("Player")) return;

		// If the bubble is already pushed, don't re-push it

		// Vector from player's position to bubble's position
		Vector2 bubblePos = transform.position;
		Vector2 playerPos = other.transform.position;
		Vector2 direction = (bubblePos - playerPos);

		// Compute how far from straight-up this direction is
		float angle = Vector2.Angle(Vector2.up, direction);

		// Decide final push direction based on angle thresholds
		Vector2 pushDir;

		float halfPushVerticalAngle = (180.0f - data.pushHorizontalAngle)/2;
		if (angle < halfPushVerticalAngle)
		{
			pushDir = Vector2.up;
		}
		else if (angle < 180 - halfPushVerticalAngle)
		{
			pushDir = Mathf.Sign(direction.x) * Vector2.right;
		}
		else
		{
			return;
		}
		Push(pushDir);
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
