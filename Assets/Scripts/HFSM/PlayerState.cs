using System;
using UnityEngine;
using UnityHFSM;

namespace HFSM
{
	public class PlayerState : AnimState<string, string>
	{
		protected MonoBehaviour mono;
		protected PlayerInput input;
		protected MovementController movementController;

		public PlayerState(
			MonoBehaviour mono,
			PlayerInput input,
			Animator animator = null,
			Action<AnimState<string, string>> onEnter = null,
			Action<AnimState<string, string>> onLogic = null,
			Action<AnimState<string, string>> onPhysics = null,
			Action<AnimState<string, string>> onExit = null,
			Func<AnimState<string, string>, bool> canExit = null,
			bool needsExitTime = false,
			bool isGhostState = false) : base(mono.GetComponent<Animator>(), onEnter, onLogic, onPhysics, onExit, canExit, needsExitTime, isGhostState)
		{
			this.mono = mono;
			this.input= input;
			this.movementController = mono.GetComponent<MovementController>();
		}
	}
}