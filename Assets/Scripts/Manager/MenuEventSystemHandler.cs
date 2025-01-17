using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class MenuEventSystemHandler : MonoBehaviour
{
	[Header("Reference")]
	public List<Selectable> selectables = new List<Selectable>();
	public UnityEvent soundEvent;
	[SerializeField] protected Selectable firstSelectable;
	[Header("Input")]
	[SerializeField] protected InputActionReference navigateReference;
	[Header("Anim")]
	[SerializeField] protected float selectAnimScale = 1.1f;
	[SerializeField] protected float selectAnimDuration = 0.1f;
	public List<Selectable> animExclusions = new List<Selectable>();

	protected Dictionary<Selectable, Vector3> oriScaleDict = new Dictionary<Selectable, Vector3>();
	protected Dictionary<Selectable, bool> animExclusionDict = new Dictionary<Selectable, bool>();

	protected Selectable lastSelectable;

	protected Tween scaleSelectTween;
	protected Tween scaleDeselectTween;

	protected virtual void Awake()
	{
		foreach (var selectable in selectables)
		{
			oriScaleDict.Add(selectable, selectable.transform.localScale);
			AddSelectionListeners(selectable);
		}
		foreach (var selectable in animExclusions)
		{
			animExclusionDict.Add(selectable, true);
		}
	}

	protected virtual void OnEnable()
	{
		if (navigateReference)
		{
			navigateReference.action.performed += OnNavigate;
		}
		foreach (var selectable in selectables)
		{
			selectable.transform.localScale = oriScaleDict[selectable];
		}
		EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
	}

	protected virtual void OnDisable()
	{
		if (navigateReference)
		{
			navigateReference.action.performed -= OnNavigate;
		}
		scaleSelectTween?.Kill(true);
		scaleDeselectTween?.Kill(true);
	}

	protected virtual void AddSelectionListeners(Selectable selectable)
	{
		EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
		if (trigger == null)
		{
			trigger = selectable.gameObject.AddComponent<EventTrigger>();
		}

		EventTrigger.Entry select = new EventTrigger.Entry { eventID = EventTriggerType.Select };
		select.callback.AddListener(OnSelect);
		EventTrigger.Entry deselect = new EventTrigger.Entry { eventID = EventTriggerType.Deselect };
		deselect.callback.AddListener(OnDeselect);
		EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
		pointerEnter.callback.AddListener(OnPointerEnter);
		EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
		pointerExit.callback.AddListener(OnPointerExit);

		trigger.triggers.Add(select);
		trigger.triggers.Add(deselect);
		trigger.triggers.Add(pointerEnter);
		trigger.triggers.Add(pointerExit);
	}

	public void OnSelect(BaseEventData eventData)
	{
		soundEvent?.Invoke();
		Selectable selectable = eventData.selectedObject.GetComponent<Selectable>();
		if (!animExclusionDict.ContainsKey(selectable))
		{
			Vector3 newScale = eventData.selectedObject.transform.localScale * selectAnimScale;
			scaleSelectTween = eventData.selectedObject.transform.DOScale(newScale, selectAnimDuration);
		}
		lastSelectable = selectable;
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if (!animExclusionDict.ContainsKey(eventData.selectedObject.GetComponent<Selectable>()))
		{

			var selectable = eventData.selectedObject.GetComponent<Selectable>();
			Vector3 scale = oriScaleDict[selectable];
			scaleDeselectTween = eventData.selectedObject.transform.DOScale(scale, selectAnimDuration);
		}
	}

	public void OnPointerEnter(BaseEventData eventData)
	{
		PointerEventData pointerEventData = eventData as PointerEventData;
		if (pointerEventData != null)
		{
			Selectable selectable = pointerEventData.pointerEnter.GetComponent<Selectable>();
			pointerEventData.selectedObject = selectable.gameObject;
		}
	}

	public void OnPointerExit(BaseEventData eventData)
	{
		PointerEventData pointerEventData = eventData as PointerEventData;
		if (pointerEventData != null)
		{
			pointerEventData.selectedObject = null;
		}
	}

	public void OnNavigate(InputAction.CallbackContext ctx)
	{
		Debug.Log("OnNavigate");
		if (EventSystem.current.currentSelectedGameObject == null && lastSelectable != null)
		{
			EventSystem.current.SetSelectedGameObject(lastSelectable.gameObject);
		}
	}
}
