using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent<ArrayList>> eventDictionary = new Dictionary<string, UnityEvent<ArrayList>>();
    private static EventManager instance;

    public static EventManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void StartListening(string eventName, UnityAction<ArrayList> listener)
    {
        UnityEvent<ArrayList> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<ArrayList>();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<ArrayList> listener)
    {
        if (instance == null) return;
        UnityEvent<ArrayList> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, ArrayList objs)
    {
        UnityEvent<ArrayList> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(objs);
        }
    }
}