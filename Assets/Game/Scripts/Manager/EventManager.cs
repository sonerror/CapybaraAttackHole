using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Events;

public enum EventType
{
    Gold
}
public static class EventManager 
{
    private static readonly Dictionary<EventType, UnityEvent<int>> intEventDictionary = new Dictionary<EventType, UnityEvent<int>>
    {
        { EventType.Gold, new UnityEvent<int>() },
    };
    public static void Subscribe(EventType eventType, UnityAction<int> action)
    {
        if (intEventDictionary.TryGetValue(eventType, out var unityEvent))
        {
            unityEvent.AddListener(action);
        }
    }
    // Unsubscription methods
    public static void Unsubscribe(EventType eventType, UnityAction<int> action)
    {
        if (intEventDictionary.TryGetValue(eventType, out var unityEvent))
        {
            unityEvent.RemoveListener(action);
        }
    }
    public static void Invoke(EventType eventType, int parameter)
    {
        if (intEventDictionary.TryGetValue(eventType, out var unityEvent))
        {
            unityEvent.Invoke(parameter);
        }
    }
}
