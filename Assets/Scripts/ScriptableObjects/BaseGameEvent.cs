using UnityEngine;
using System;
using System.Collections.Generic;

public class BaseGameEvent<T> : ScriptableObject {
    private readonly List<Action<T>> listeners = new();

    public virtual void Invoke(T data) {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i]?.Invoke(data);
    }

    public virtual void RegisterListener(Action<T> listener) {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public virtual void UnregisterListener(Action<T> listener) {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }
}
