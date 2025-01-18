using System;
using UnityEngine;
using UnityHFSM;

namespace HFSM
{
	public class PlayerStateBubble : PlayerState
	{
		public bool immediately;
		private GameObject _bubble;

		public PlayerStateBubble(
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

		public override void OnEnter()
		{
			if (_bubble != null)
			{
				_bubble.GetComponent<BubbleBehaviour>().destroyBubble();
			}
			_bubble = mono.GetComponent<BubbleSpawner>().SpawnBubble();
			base.OnEnter();
		}
	}
}
