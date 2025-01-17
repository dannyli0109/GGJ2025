using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[Serializable]
public class InputBuffer
{
	public string name;
	public float time = 0.1f;
	[HideInInspector]
	public bool enable;
	[HideInInspector]
	public float value;
}

public class PlayerInput : MonoBehaviour
{
	PlayerInputActions inputActions;

	public float horizontal => inputActions.GamePlay.Horizontal.ReadValue<float>();
	public Vector2 aim => inputActions.GamePlay.Aim.ReadValue<Vector2>();
	public List<InputBuffer> bufferList;

	public bool Attack => inputActions.GamePlay.Attack.WasPressedThisFrame(); // ¹¥»÷
	public bool Switch => inputActions.GamePlay.Switch.WasPressedThisFrame(); // ÇÐ»»ÎäÆ÷
	public bool Reload => inputActions.GamePlay.Reload.WasPressedThisFrame(); // ×°Ìîµ¯Ò©
	public bool Sprint => inputActions.GamePlay.Sprint.WasPressedThisFrame(); // ³å´Ì
	public bool Down => inputActions.GamePlay.Down.IsPressed(); // ÏÂ¶×
	public bool Move => horizontal != 0f;

	Dictionary<string, InputBuffer> bufferDict;
	Dictionary<string, Coroutine> bufferCoroutineDict;

	void Awake()
	{
		inputActions = new PlayerInputActions();
		bufferDict = new Dictionary<string, InputBuffer>();
		bufferCoroutineDict = new Dictionary<string, Coroutine>();
		foreach (var buffer in bufferList)
		{
			bufferDict.Add(buffer.name, buffer);
		}
	}

	private void OnEnable()
	{
		EnableGameplayInputs();
	}

	private void OnDisable()
	{
		DisableGameplayInputs();
	}

	public void EnableGameplayInputs()
	{
		inputActions.GamePlay.Enable();
		inputActions.GamePlay.Jump.performed += OnJumpPerformed;
		inputActions.GamePlay.Sprint.performed += OnSprintPerformed;
	}

	public void DisableGameplayInputs()
	{
		inputActions.GamePlay.Disable();
		inputActions.GamePlay.Jump.performed -= OnJumpPerformed;
		inputActions.GamePlay.Sprint.performed -= OnSprintPerformed;
	}

	private void OnJumpPerformed(InputAction.CallbackContext ctx)
	{
		if (ctx.interaction is PressInteraction)
		{
			SetInputBufferTimer("Jump");
		}
	}

	private void OnSprintPerformed(InputAction.CallbackContext ctx)
	{
		if (ctx.interaction is PressInteraction)
		{
			if (horizontal != 0)
			{
				SetInputBufferTimer("Sprint", Mathf.Sign(horizontal));
			}
		}
	}

	public InputBuffer GetBuffer(string name)
	{
		InputBuffer buffer = null;
		bufferDict.TryGetValue(name, out buffer);
		return buffer;
	}

	public bool HasBuffer(string name)
	{
		InputBuffer buffer = GetBuffer(name);
		if (buffer != null)
		{
			return buffer.enable;
		}
		return false;
	}

	public void UseBuffer(string name)
	{
		InputBuffer buffer = GetBuffer(name);
		if (buffer != null)
		{
			buffer.enable = false;
		}
	}

	public void SetInputBufferTimer(string name, float value = 0)
	{
		StopBufferCoroutine(name);
		var coroutine = StartCoroutine(InputBufferCoroutine(name, value));
		bufferCoroutineDict[name] = coroutine;
	}

	public void StopBufferCoroutine(string name)
	{
		bufferCoroutineDict.TryGetValue(name, out Coroutine coroutine);
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			bufferCoroutineDict.Remove(name);
		}
	}

	IEnumerator InputBufferCoroutine(string name, float value = 0)
	{
		var buffer = GetBuffer(name);
		buffer.value = value;
		buffer.enable = true;
		yield return new WaitForSeconds(buffer.time);
		buffer.enable = false;
		bufferCoroutineDict.Remove(name);
	}
}
