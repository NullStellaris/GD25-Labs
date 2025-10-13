using System;
using System.Collections.Generic;
using UnityEngine;

public struct Void { }

[CreateAssetMenu(
    fileName = "GameEvent",
    menuName = "Scriptable Objects/GameEvent"
)]
public class GameEvent : BaseGameEvent<Void> {
    private readonly List<Action> voidListeners = new();

    public override void Invoke(Void _) {
        base.Invoke(_);
        for (int i = voidListeners.Count - 1; i >= 0; i--)
            voidListeners[i]?.Invoke();
    }

    public void Invoke() => Invoke(new Void());

    public void RegisterListener(Action listener) {
        if (!voidListeners.Contains(listener))
            voidListeners.Add(listener);
    }

    public void UnregisterListener(Action listener) {
        if (voidListeners.Contains(listener))
            voidListeners.Remove(listener);
    }
}
