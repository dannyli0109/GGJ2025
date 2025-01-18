using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class MovementController : MonoBehaviour
{
	public PlayerMovementData movementData;
	public ContactFilter2D groundContactFilter;
	public Vector2 velocity => rig.velocity;
	public bool jumpEnable => jumpCount > 0;
	public bool sprintEnable = true;
	[HideInInspector] public Vector3 spawnPos;
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

	[HideInInspector] public bool inCoyoteTime = false;
	[HideInInspector] public bool isOnGround => IsOnGround();

	PlayerInput input;
	BoxCollider2D col;
	Rigidbody2D rig;
	Camera mainCamera;

	#region System Function
	private void Awake()
	{
		input = GetComponent<PlayerInput>();
		col = GetComponent<BoxCollider2D>();
		rig = GetComponent<Rigidbody2D>();
		mainCamera = Camera.main;
		landJumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * rig.gravityScale * movementData.landJumpHeight);
		//airJumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * rig.gravityScale * movementData.airJumpHeight);
		ResetJumpCount();
		RecordSpawnPos();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Death"))
		{
			gameObject.transform.position = spawnPos;
		}
	}

	private void OnEnable()
	{
		//RecordSpawnPos();
	}

	private void OnDisable()
	{
		transform.position = spawnPos;
	}
	#endregion

	public void ResetJumpCount()
	{
		JumpCount = movementData.jumpCount;
	}

	public void Jump()
	{
		RecordSpawnPos();
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
		Vector3 scale = transform.localScale;
		if (input.horizontal * scale.x < 0)
		{
			scale.x = Mathf.Sign(input.horizontal) * Mathf.Abs(scale.x);
			transform.localScale = scale;
		}
	}

	//public void SprintCountDown()
	//{
	//	StartCoroutine(SprintCountDownProgress(movementData.sprintCD));
	//}

	public bool IsOnGround()
	{
		return rig.IsTouching(groundContactFilter);
	}

	IEnumerator SprintCountDownProgress(float t)
	{
		sprintEnable = false;
		yield return new WaitForSeconds(t);
		sprintEnable = true;
	}

	IEnumerator DelayFunc(UnityAction action, float t)
	{
		yield return new WaitForSeconds(t);
		action();
	}
}
