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

        public AudioClip jumpSound;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            movementController = GetComponent<MovementController>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            InitLowerFsm();
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

        void InitLowerFsm()
        {
            var normalFsm = new HybridStateMachine(afterOnLogic: AfterOnLogic, afterOnPhysics: AfterOnPhysics);

            normalFsm.AddState("Fall", new PlayerStateFall(this, input));
            normalFsm.AddState("Idle", new PlayerStateIdle(this, input));
            normalFsm.AddState("Run", new PlayerStateRun(this, input));
            //normalFsm.AddState("CoyoteTime", new PlayerStateCoyoteTime(this, input));
            normalFsm.AddState("Jump", new PlayerStateJump(this, input));
            normalFsm.AddState("Rise", new PlayerStateRise(this, input));
            normalFsm.AddState("Bubble", new PlayerStateBubble(this, input));
            //normalFsm.AddState("AirJump", new PlayerStateJump(this, input));

            //normalFsm.AddTransition("Fall", "AirJump", condition: CheckJump);
            normalFsm.AddTransition("Fall", "Idle", condition: t => movementController.isOnGround, onTransition: t => movementController.RecordSpawnPos());
            normalFsm.AddTwoWayTransition("Idle", "Run", t => input.Move);//TODO
            normalFsm.AddTransition("Idle", "Jump", condition: CheckJump);
            normalFsm.AddTransition("Idle", "Bubble", condition: t => input.Interact);
            normalFsm.AddTransition("Idle", "Fall", condition: CheckOnAir, onTransition: OnTransitionPlatformToFall);
            normalFsm.AddTransition("Idle", "Fall", condition: t => input.Down && input.HasBuffer("Jump"), onTransition: OnTransitionPlatformToFall);
            //normalFsm.AddTransition("Run", "CoyoteTime", condition: CheckOnAir);
            normalFsm.AddTransition("Run", "Jump", condition: CheckJump);
            normalFsm.AddTransition("Run", "Bubble", condition: t => input.Interact);
            normalFsm.AddTransition("Run", "Fall", condition: CheckOnAir, onTransition: OnTransitionPlatformToFall);
            //normalFsm.AddTransition("CoyoteTime", "Jump", condition: CheckJump);
            //normalFsm.AddTransition(new TransitionAfter("CoyoteTime", "Fall", movementController.movementData.coyoteTime, onTransition: OnTransitionPlatformToFall));
            normalFsm.AddTransition("Jump", "Rise", condition: t => movementController.velocity.y > 0);
            normalFsm.AddTransition("Rise", "Fall", condition: t => movementController.velocity.y <= 0);
            normalFsm.AddTransition(new TransitionAfter("Bubble", "Idle", 0.2f));
            //normalFsm.AddTransition("Rise", "AirJump", condition: CheckJump);
            //normalFsm.AddTransition("AirJump", "Rise", condition: t => movementController.velocity.y > 0);

            lowerFsm = new StateMachine();
            lowerFsm.AddState("Normal", normalFsm);
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