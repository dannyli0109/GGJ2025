using System;
using System.Collections;
using DG.Tweening;
using HFSM;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class MovementController : NetworkBehaviour
{
	public PlayerMovementData movementData;
	public ContactFilter2D groundContactFilter;
	public Vector2 velocity => rig.velocity;
	public bool jumpEnable => jumpCount > 0;
	[HideInInspector] public Vector3 spawnPos;
	public AudioClip jumpSound;

	AudioSource audioSource;
	private int jumpCount;
	public int JumpCount
	{
		get
		{
			return jumpCount;
		}
		set
		{
			jumpCount = Mathf.Max(0, value);
		}
	}
	float landJumpSpeed;
	//float airJumpSpeed;

	[HideInInspector]
	[SyncVar(hook = nameof(OnTowardChanged))]
	public bool towardRight = true;
	[HideInInspector] public bool inCoyoteTime = false;
	[HideInInspector] public bool isOnGround => IsOnGround();

	PlayerInput input;
	BoxCollider2D col;
	Rigidbody2D rig;
	Camera mainCamera;
	[HideInInspector] public GroundDetector groundDetector;

	#region System Function
	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		input = GetComponent<PlayerInput>();
		col = GetComponent<BoxCollider2D>();
		rig = GetComponent<Rigidbody2D>();
		groundDetector = GetComponentInChildren<GroundDetector>();
		mainCamera = Camera.main;
		landJumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * rig.gravityScale * movementData.landJumpHeight);
		ResetJumpCount();
		RecordSpawnPos();
	}

	private void OnEnable()
	{
		GameManager.Instance.AddTarget(transform);
	}

	private void OnDisable()
	{
		GameManager.Instance.RemoveTarget(transform);
	}

	//public override void OnStartLocalPlayer()
	//{
	//	base.OnStartLocalPlayer();
	//	Debug.Log("OnStartLocalPlayer");
	//	GameManager.Instance.AddTarget(transform);
	//}

	//public override void OnStopLocalPlayer()
	//{
	//	base.OnStopLocalPlayer();
	//	Debug.Log("OnStopLocalPlayer");
	//	GameManager.Instance.RemoveTarget(transform);
	//}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Death"))
		{
			if (isLocalPlayer)
			{
				gameObject.transform.position = spawnPos;
				CmdDeath();
			}
		}
	}
	#endregion

	[Command]
	public void CmdDeath()
	{
		RpcOnDeath();
	}

	[ClientRpc]
	void RpcOnDeath()
	{
		var children = GetComponentsInChildren<SpriteRenderer>();
		foreach (var child in children)
		{
			child.color = new Color(child.color.r, child.color.g, child.color.b, 0);
			child.DOFade(1, 1f);
		}
	}

	public void ResetJumpCount()
	{
		JumpCount = movementData.jumpCount;
	}

	public void Jump()
	{
		float velocityX = rig.velocity.x;
		//float velocityX = input.horizontal*movementData.speed;
		//if (input.Move && input.horizontal*rig.velocity.x<0)
		//{
		//    velocityX *= -1;
		//}
		if (jumpCount == movementData.jumpCount)
		{
			SetVelocity(velocityX, landJumpSpeed);
		}
		//else
		//{
		//	if (rig.velocity.y > 0)
		//	{
		//		//上升过程
		//		SetVelocity(velocityX, airJumpSpeed);
		//	}
		//	else
		//	{
		//		//下落过程
		//		SetVelocity(velocityX, airJumpSpeed);
		//	}
		//}
	}

	public void RecordSpawnPos()
	{
		spawnPos = transform.position;
	}

	public void SetFallGravityScale()
	{
		rig.gravityScale = movementData.fallGravityScale;
	}

	public void ResetGravityScale()
	{
		rig.gravityScale = 1;
	}

	public void LimitFallSpeed()
	{
		if (rig.velocity.y < -movementData.maxFallSpeed)
		{
			rig.velocity = new Vector2(rig.velocity.x, -movementData.maxFallSpeed);
		}
	}

	public void SetVelocityX(float v)
	{
		Vector2 velocity = rig.velocity;
		velocity.x = v;
		rig.velocity = velocity;
	}

	public void SetVelocityY(float v)
	{
		Vector2 velocity = rig.velocity;
		velocity.y = v;
		rig.velocity = velocity;
	}

	public void SetVelocity(float x, float y)
	{
		Vector2 v = new Vector2(x, y);
		rig.velocity = v;
	}

	public void SetVelocity(Vector2 v)
	{
		rig.velocity = v;
	}

	public void SetGravityScale(float scale)
	{
		rig.gravityScale = scale;
	}

	// 更新朝向
	public void UpdateToward()
	{
		if (isLocalPlayer)
		{
			Vector3 scale = transform.localScale;
			if (input.horizontal * scale.x < 0)
			{
				scale.x = Mathf.Sign(input.horizontal) * Mathf.Abs(scale.x);
				transform.localScale = scale;
				CmdUpdateToward(transform.localScale.x > 0);
			}
		}
	}

	public void OnJump()
	{
		audioSource.clip = jumpSound;
		audioSource.Play();
	}

	[Command]
	void CmdUpdateToward(bool right)
	{
		towardRight = right;
	}

	void OnTowardChanged(bool oldToward, bool newToward)
	{
		float scale_x = Mathf.Abs(transform.localScale.x);
		scale_x = newToward ? scale_x : -scale_x;
		Vector3 scale = transform.localScale;
		scale.x = scale_x;
		transform.localScale = scale;
	}

	//public void SprintCountDown()
	//{
	//	StartCoroutine(SprintCountDownProgress(movementData.sprintCD));
	//}

	public bool IsOnGround()
	{
		return rig.IsTouching(groundContactFilter);
	}

	IEnumerator DelayFunc(UnityAction action, float t)
	{
		yield return new WaitForSeconds(t);
		action();
	}
}
