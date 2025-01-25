using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class BubbleDownOnStand : NetworkBehaviour
{
	[SyncVar(hook = nameof(OnRigBodyTypeChanged))]
	int rigBodyType;
	[SyncVar(hook = nameof(OnRigGravityScaleChanged))]
	float rigGravityScale;
	private Rigidbody2D _rb;
	public float gravityScale = 0.2f;
	private bool isStandOn = false;
	private List<GameObject> gameObjects = new List<GameObject>();
	private int _count = 0;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		if (isServer)
		{
			rigBodyType = (int)_rb.bodyType;
			rigGravityScale = _rb.gravityScale;
		}
	}

	void OnRigBodyTypeChanged(int _, int newValue)
	{
		_rb.bodyType = (RigidbodyType2D)newValue;
	}

	void OnRigGravityScaleChanged(float _, float newValue)
	{
		_rb.gravityScale = newValue;
	}

	[Command(requiresAuthority = false)]
	public void AddPlayer(GameObject gameObject)
	{
		bool found = false;
		for (int i = 0; i < gameObjects.Count; i++)
		{
			if (gameObjects[i] == gameObject)
			{
				found = true;
				break;
			}
		}
		if (!found)
		{
			gameObjects.Add(gameObject);
		}
		// down force
		rigBodyType = (int)RigidbodyType2D.Dynamic;
		rigGravityScale = gravityScale * gameObjects.Count;
	}

	[Command(requiresAuthority = false)]
	public void RemovePlayer(GameObject gameObject)
	{
		for (int i = 0; i < gameObjects.Count; i++)
		{
			if (gameObjects[i] == gameObject)
			{
				gameObjects.RemoveAt(i);
				break;
			}
		}

		rigGravityScale = gravityScale * gameObjects.Count;
		if (gameObjects.Count == 0)
		{
			rigBodyType = (int)RigidbodyType2D.Kinematic;
		}
	}

	[ServerCallback]
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			AddPlayer(collision.gameObject);
		}
	}

	[ServerCallback]
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			RemovePlayer(collision.gameObject);
		}
	}

	[ServerCallback]
	private void Update()
	{
		if (gameObjects.Count > 0)
		{
			_rb.velocity = new Vector2(0, -rigGravityScale);
		}
		else
		{
			_rb.velocity = Vector2.zero;
		}
	}
}
