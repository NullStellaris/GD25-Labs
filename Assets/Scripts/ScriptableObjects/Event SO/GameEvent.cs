using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Game Event", menuName = "Events/GameEvent")]
public class GameEvent : ScriptableObject {
    private readonly List<Action> listeners = new();

    public void Invoke() {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i]?.Invoke();
    }

    public void RegisterListener(Action listener) {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void UnregisterListener(Action listener) {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }
}
