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
	protected PlayerInputActions inputActions;

	public virtual float horizontal => inputActions.GamePlay.Horizontal.ReadValue<float>();
	public List<InputBuffer> bufferList;

	public virtual bool Interact => inputActions.GamePlay.Interact.WasPressedThisFrame(); // ¹¥»÷
	public virtual bool Down => inputActions.GamePlay.Down.IsPressed(); // ÏÂ¶×
	public bool Move => horizontal != 0f;

	protected Dictionary<string, InputBuffer> bufferDict;
	protected Dictionary<string, Coroutine> bufferCoroutineDict;

	protected virtual void Awake()
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

	public virtual void EnableGameplayInputs()
	{
		inputActions.GamePlay.Enable();
		inputActions.GamePlay.Jump.performed += OnJumpPerformed;
	}

	public virtual void DisableGameplayInputs()
	{
		inputActions.GamePlay.Disable();
		inputActions.GamePlay.Jump.performed -= OnJumpPerformed;
	}

	protected void OnJumpPerformed(InputAction.CallbackContext ctx)
	{
		if (ctx.interaction is PressInteraction)
		{
			SetInputBufferTimer("Jump");
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
