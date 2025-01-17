using System;
using UnityEngine;
using UnityHFSM;

namespace HFSM
{
	public class PlayerFSM : MonoBehaviour
	{
		[HideInInspector] public PlayerInput input;
		[HideInInspector] public MovementController movementController;
		[HideInInspector] public Animator animator;

		public StateMachine upperFsm;
		public StateMachine lowerFsm;

		private void Awake()
		{
			input = GameManager.Instance.input;
			movementController = GetComponent<MovementController>();
			animator = GetComponent<Animator>();
		}

		private void Start()
		{
			InitLowerFsm();
			//InitUpperFsm();
		}

		private void Update()
		{
			//upperFsm.OnLogic();
			lowerFsm.OnLogic();
		}

		private void FixedUpdate()
		{
			//upperFsm.OnPhysics();
			lowerFsm.OnPhysics();
		}

		void InitUpperFsm()
		{
			upperFsm = new StateMachine();
			upperFsm.AddState("Aim");
			upperFsm.AddState("Fire");
			upperFsm.AddState("ReloadWeapon");
			upperFsm.AddState("SwitchWeapon");
			upperFsm.AddState("SwitchWeapon");
			upperFsm.AddState("None");

			upperFsm.Init();
		}

		void InitLowerFsm()
		{
			var normalFsm = new HybridStateMachine(afterOnLogic: AfterOnLogic, afterOnPhysics: AfterOnPhysics);
			//normalFsm.AddState("None", isGhostState: true);
			//normalFsm.AddState("Land");
			normalFsm.AddState("Fall", new PlayerStateFall(this, input));
			normalFsm.AddState("Idle", new PlayerStateIdle(this, input));
			normalFsm.AddState("Run", new PlayerStateRun(this, input));
			normalFsm.AddState("CoyoteTime", new PlayerStateCoyoteTime(this, input));
			normalFsm.AddState("Jump", new PlayerStateJump(this, input));
			//normalFsm.AddState("Fall");
			//normalFsm.AddState("Crouch", new AnimState(animator));//TODO
			normalFsm.AddState("Rise", new PlayerStateRise(this, input));
			normalFsm.AddState("AirJump", new PlayerStateJump(this, input));
			//normalFsm.AddState("Sprint", new PlayerStateSprint(this, input));
			//normalFsm.AddTransition("None", "Idle", t => !input.Move);
			//normalFsm.AddTransition("None", "Run", t => input.Move);
			normalFsm.AddTransition("Fall", "AirJump", condition: CheckJump);
			normalFsm.AddTransition("Fall", "Idle", condition: t => movementController.isOnGround);
			normalFsm.AddTwoWayTransition("Idle", "Run", t => input.Move);//TODO
			normalFsm.AddTransition("Idle", "Jump", condition: CheckJump);
			normalFsm.AddTransition("Idle", "Fall", condition: CheckOnAir, onTransition: OnTransitionPlatformToFall);
			normalFsm.AddTransition("Idle", "Fall", condition: t => input.Down && input.HasBuffer("Jump"), onTransition: OnTransitionPlatformToFall);
			//normalFsm.AddTwoWayTransition("Idle", "Crouch", t => input.Down);
			normalFsm.AddTransition("Run", "CoyoteTime", condition: CheckOnAir);
			normalFsm.AddTransition("Run", "Jump", condition: CheckJump);
			normalFsm.AddTransition("Run", "Fall", condition: CheckOnAir, onTransition: OnTransitionPlatformToFall);
			normalFsm.AddTransition("CoyoteTime", "Jump", condition: CheckJump);
			normalFsm.AddTransition(new TransitionAfter("CoyoteTime", "Fall", movementController.movementData.coyoteTime, onTransition: OnTransitionPlatformToFall));
			normalFsm.AddTransition("Jump", "Rise", condition: t => movementController.velocity.y > 0);
			normalFsm.AddTransition("Rise", "Fall", condition: t => movementController.velocity.y <= 0);
			normalFsm.AddTransition("Rise", "AirJump", condition: CheckJump);
			normalFsm.AddTransition("AirJump", "Rise", condition: t => movementController.velocity.y > 0);

			//normalFsm.AddTransitionFromAny("Sprint", condition: t => movementController.sprintEnable && input.HasBuffer("Sprint"));
			//normalFsm.AddTransition(new TransitionAfter("Sprint", "Fall", movementController.movementData.sprintTime));

			//var airFsm = new HybridStateMachine(afterOnLogic: OnAfterLogic, afterOnPhysics: OnAfterPhysicsAir);
			//airFsm.AddState("None", isGhostState: true);
			//airFsm.AddState("Rise", new PlayerStateRise(this, input));
			//airFsm.AddState("Fall", new PlayerStateFall(this, input));
			//airFsm.AddState("AirJump", new PlayerStateJump(this, input));
			//airFsm.AddTransition("None", "Rise", t => movementController.velocity.y > 0);
			//airFsm.AddTransition("None", "Fall", t => movementController.velocity.y <= 0, onTransition: t => movementController.JumpCount--);
			//airFsm.AddTransition("Rise", "Fall", t => movementController.velocity.y <= 0);
			//airFsm.AddTransition("AirJump", "Rise", t => movementController.velocity.y > 0);
			//airFsm.AddTransition("Rise", "AirJump", CheckJump);
			//airFsm.AddTransition("Fall", "AirJump", CheckJump);
			//airFsm.AddTransitionFromAny(new Transition("", "Sprint", t => movementController.sprintEnable && input.HasBuffer("Sprint")));
			//airFsm.AddTransition(new TransitionAfter("Sprint", "Fall", movementController.movementData.sprintTime));

			//var etcFsm = new HybridStateMachine(
			//	beforeOnLogic: state => { },
			//	needsExitTime: false
			//);
			//etcFsm.AddState("KnockedDown");
			//etcFsm.AddState("Lie");
			//etcFsm.AddState("StandUp");
			//etcFsm.AddState("Death");
			//etcFsm.AddState("Freeze");

			lowerFsm = new StateMachine();
			lowerFsm.AddState("Normal", normalFsm);
			//lowerFsm.AddState("Etc", etcFsm);
			//lowerFsm.AddTransitionFromAny(new Transition("", "Death", t => true));//TODO
			//lowerFsm.AddTransition("Ground", "Air", t => !movementController.isOnGround && !movementController.inCoyoteTime);
			//lowerFsm.AddTransition("Air", "Ground", t => movementController.isOnGround);
			//lowerFsm.AddTransition("Ground", "Sprint", t => movementController.sprintEnable && input.HasBuffer("Sprint"));
			//lowerFsm.AddTransition("Air", "Sprint", t => movementController.sprintEnable && input.HasBuffer("Sprint"));
			//lowerFsm.AddTransition(new TransitionAfter("Sprint", "Air", movementController.movementData.sprintTime, onTransition: t => movementController.JumpCount = 0));
			//lowerFsm.AddTwoWayTransition("Ground", "Etc", t => !movementController.enabled);
			//lowerFsm.AddTransition("Air", "Etc", t => !movementController.enabled);
			lowerFsm.Init();
		}

		void AfterOnPhysics(StateMachine<string, string, string> sm)
		{
			if (movementController.isOnGround)
			{
				AfterOnPhysicsGround(sm);
			}
			else
			{
				AfterOnPhysicsAir(sm);
			}
		}

		void AfterOnPhysicsGround(StateMachine<string, string, string> sm)
		{
			PlayerMovementData movementData = movementController.movementData;
			float speed = movementData.speed;
			if (movementData.noAccelerateOnGround)
			{
				movementController.SetVelocityX(speed * input.horizontal);
			}
			else
			{
				float acceleration = input.horizontal == 0 ? movementData.deceleration : movementData.acceleration;
				float currentSpeed = movementController.velocity.x;
				currentSpeed = Mathf.MoveTowards(currentSpeed, speed * input.horizontal, acceleration * Time.fixedDeltaTime);
				movementController.SetVelocityX(currentSpeed);
			}
		}

		void AfterOnPhysicsAir(StateMachine<string, string, string> sm)
		{
			float speed = movementController.movementData.speed;
			float acceleration = movementController.movementData.airAcceleration;
			float currentSpeed = movementController.velocity.x;
			currentSpeed = Mathf.MoveTowards(currentSpeed, speed * input.horizontal, acceleration * Time.fixedDeltaTime);
			movementController.SetVelocityX(currentSpeed);
		}

		void AfterOnLogic(StateMachine<string, string, string> sm)
		{
			movementController.UpdateToward();
		}

		bool CheckOnAir(Transition<string> t)
		{
			return !movementController.isOnGround;
		}

		bool CheckJump(Transition<string> t)
		{
			return movementController.jumpEnable && !input.Down && input.HasBuffer("Jump");
		}

		bool CheckCrossPlatform(Transition<string> t)
		{
			return input.Down && input.HasBuffer("Jump");
		}

		void OnTransitionPlatformToFall(Transition<string> t)
		{
			movementController.JumpCount--;
		}

		void OnTransitionPlatformToFall(TransitionAfter<string> t)
		{
			movementController.JumpCount--;
		}
	}
}