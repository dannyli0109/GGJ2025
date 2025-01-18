using System;
using UnityEngine;
using UnityHFSM;

namespace HFSM
{
	public class PlayerStateFall : PlayerState
	{
		public PlayerStateFall(
			MonoBehaviour mono,
			PlayerInput input,
			Animator animator = null,
			Action<AnimState<string, string>> onEnter = null,
			Action<AnimState<string, string>> onLogic = null,
			Action<AnimState<string, string>> onPhysics = null,
			Action<AnimState<string, string>> onExit = null,
			Func<AnimState<string, string>, bool> canExit = null,
			bool needsExitTime = false,
			bool isGhostState = false) : base(mono, input, animator, onEnter, onLogic, onPhysics, onExit, canExit, needsExitTime, isGhostState)
		{
		}

		public override void OnPhysics()
		{
			base.OnPhysics();
			movementController.SetFallGravityScale();
			movementController.LimitFallSpeed();
		}
	}
}
