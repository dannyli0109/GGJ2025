using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHFSM
{
	public class AnimState<TStateId, TEvent> : ActionState<TStateId, TEvent>
	{
		protected Action<AnimState<TStateId, TEvent>> onEnter;
		protected Action<AnimState<TStateId, TEvent>> onLogic;
		protected Action<AnimState<TStateId, TEvent>> onPhysics;
		protected Action<AnimState<TStateId, TEvent>> onExit;
		protected Func<AnimState<TStateId, TEvent>, bool> canExit;

		public ITimer timer;

		protected Animator animator;
		protected int animHash;
		public bool isAnimationFinished
		{
			get
			{
				var info = animator.GetCurrentAnimatorStateInfo(0);
				return info.normalizedTime >= 1;
			}
		}

		/// <summary>
		/// Initialises a new instance of the AnimState class.
		/// </summary>
		/// <param name="onEnter">A function that is called when the state machine enters this state.</param>
		/// <param name="onLogic">A function that is called by the logic function of the state machine if this
		/// 	state is active.</param>
		/// <param name="onExit">A function that is called when the state machine exits this state.</param>
		/// <param name="canExit">(Only if needsExitTime is true):
		/// 	Function that determines if the state is ready to exit (true) or not (false).
		/// 	It is called OnExitRequest and on each logic step when a transition is pending.</param>
		/// <param name="needsExitTime">Determines if the state is allowed to instantly
		/// 	exit on a transition (false), or if the state machine should wait until the state is ready for a
		/// 	state change (true).</param>
		public AnimState(
				Animator animator,
				Action<AnimState<TStateId, TEvent>> onEnter = null,
				Action<AnimState<TStateId, TEvent>> onLogic = null,
				Action<AnimState<TStateId, TEvent>> onPhysics = null,
				Action<AnimState<TStateId, TEvent>> onExit = null,
				Func<AnimState<TStateId, TEvent>, bool> canExit = null,
				bool needsExitTime = false,
				bool isGhostState = false) : base(needsExitTime, isGhostState)
		{
			this.animator = animator;
			this.onEnter = onEnter;
			this.onLogic = onLogic;
			this.onPhysics = onPhysics;
			this.onExit = onExit;
			this.canExit = canExit;
			timer = new Timer();
		}

		public override void Init()
		{
			animHash = Animator.StringToHash(name.ToString());
		}

		public override void OnEnter()
		{
			timer.Reset();

			if (animator.HasState(0, animHash))
			{
				animator.CrossFade(animHash, 0.2f);
			}

			onEnter?.Invoke(this);
		}

		public override void OnLogic()
		{
			onLogic?.Invoke(this);

			// Check whether the state is ready to exit after calling onLogic, as it may trigger a transition.
			// Calling onLogic beforehand would lead to invalid behaviour as it would be called, even though this state
			// is not active anymore.
			if (needsExitTime && canExit != null && fsm.HasPendingTransition && canExit(this))
			{
				fsm.StateCanExit();
			}
		}

		public override void OnPhysics()
		{
			onPhysics?.Invoke(this);
		}

		public override void OnExit()
		{
			onExit?.Invoke(this);
		}

		public override void OnExitRequest()
		{
			if (canExit != null && canExit(this))
			{
				fsm.StateCanExit();
			}
		}
	}

	/// <inheritdoc />
	public class AnimState<TStateId> : AnimState<TStateId, string>
	{
		/// <inheritdoc />
		public AnimState(
			Animator animator,
			Action<AnimState<TStateId, string>> onEnter = null,
			Action<AnimState<TStateId, string>> onLogic = null,
			Action<AnimState<TStateId, string>> onPhysics = null,
			Action<AnimState<TStateId, string>> onExit = null,
			Func<AnimState<TStateId, string>, bool> canExit = null,
			bool needsExitTime = false,
			bool isGhostState = false)
			: base(
				animator,
				onEnter,
				onLogic,
				onPhysics,
				onExit,
				canExit,
				needsExitTime: needsExitTime,
				isGhostState: isGhostState)
		{
		}
	}

	/// <inheritdoc />
	public class AnimState : AnimState<string, string>
	{
		/// <inheritdoc />
		public AnimState(
			Animator animator,
			Action<AnimState<string, string>> onEnter = null,
			Action<AnimState<string, string>> onLogic = null,
			Action<AnimState<string, string>> onPhysics = null,
			Action<AnimState<string, string>> onExit = null,
			Func<AnimState<string, string>, bool> canExit = null,
			bool needsExitTime = false,
			bool isGhostState = false)
			: base(
				animator,
				onEnter,
				onLogic,
				onPhysics,
				onExit,
				canExit,
				needsExitTime: needsExitTime,
				isGhostState: isGhostState)
		{
		}
	}
}
