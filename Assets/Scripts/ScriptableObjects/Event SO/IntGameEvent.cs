using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Int Game Event", menuName = "Events/GameEvent (int)")]
public class IntGameEvent : ScriptableObject {
    private readonly List<Action<int>> listeners = new();

    public void Invoke(int value) {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i]?.Invoke(value);
    }

    public void RegisterListener(Action<int> listener) {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void UnregisterListener(Action<int> listener) {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }
}
