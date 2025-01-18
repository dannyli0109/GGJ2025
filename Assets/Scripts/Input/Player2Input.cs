using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Player2Input : PlayerInput
{
	protected Player2InputActions inputActions2;

	public override float horizontal => inputActions2.GamePlay.Horizontal.ReadValue<float>();

	public override bool Interact => inputActions2.GamePlay.Interact.WasPressedThisFrame(); // ¹¥»÷
	public override bool Down => inputActions2.GamePlay.Down.IsPressed(); // ÏÂ¶×

	protected override void Awake()
	{
		inputActions2 = new Player2InputActions();
		bufferDict = new Dictionary<string, InputBuffer>();
		bufferCoroutineDict = new Dictionary<string, Coroutine>();
		foreach (var buffer in bufferList)
		{
			bufferDict.Add(buffer.name, buffer);
		}
	}

	public override void EnableGameplayInputs()
	{
		inputActions2.GamePlay.Enable();
		inputActions2.GamePlay.Jump.performed += OnJumpPerformed;
	}

	public override void DisableGameplayInputs()
	{
		inputActions2.GamePlay.Disable();
		inputActions2.GamePlay.Jump.performed -= OnJumpPerformed;
	}
}
