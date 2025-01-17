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
	float airJumpSpeed;

	[HideInInspector] public bool towardRight = true;
	[HideInInspector] public bool inCoyoteTime = false;
	//[HideInInspector] public bool isOnGround => rig.velocity.y == 0 && groundDetector.groundCollider != null;
	[HideInInspector] public bool isOnGround => IsOnGround();

	PlayerInput input;
	BoxCollider2D col;
	Rigidbody2D rig;
	Camera mainCamera;
	GroundDetector groundDetector;

	#region System Function
	private void Awake()
	{
		input = GameManager.Instance.input;
		col = GetComponent<BoxCollider2D>();
		rig = GetComponent<Rigidbody2D>();
		groundDetector = GetComponentInChildren<GroundDetector>();
		mainCamera = Camera.main;
		landJumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * rig.gravityScale * movementData.landJumpHeight);
		airJumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * rig.gravityScale * movementData.airJumpHeight);
		ResetJumpCount();
	}
	#endregion

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
		else
		{
			if (rig.velocity.y > 0)
			{
				//上升过程
				SetVelocity(velocityX, airJumpSpeed);
			}
			else
			{
				//下落过程
				SetVelocity(velocityX, airJumpSpeed);
			}
		}
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
		Vector2 pos = mainCamera.WorldToScreenPoint(transform.position);
		Vector2 dir = input.aim - pos;
		Vector3 scale = transform.localScale;
		if (dir.x * scale.x < 0)
		{
			scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
			transform.localScale = scale;
			towardRight = dir.x > 0;
		}
	}

	public void SprintCountDown()
	{
		StartCoroutine(SprintCountDownProgress(movementData.sprintCD));
	}

	public bool CanCrossDown()
	{
		groundDetector.Detect();
		return isOnGround && groundDetector.groundCollider.CompareTag("Platform");
	}

	public void CrossDownPlatform()
	{
		if (CanCrossDown())
		{
			Collider2D platformCollider = groundDetector.groundCollider;
			Physics2D.IgnoreCollision(col, platformCollider, true);
			StartCoroutine(DelayFunc(() => Physics2D.IgnoreCollision(col, platformCollider, false), 0.5f));
		}
	}

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
