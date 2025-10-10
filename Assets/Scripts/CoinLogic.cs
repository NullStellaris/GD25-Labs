using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class CoinLogic : ItemLogic {
    public Animator coinAnimator;

    // SO Events
    // send
    public IntGameEvent ScoreGain;
    // recv
    public GameEvent GlobalReset;
    void Start() {
        // Register SO Listeners
        GlobalReset.RegisterListener(OnReset);
        uses = quantity;
    }

    override public void OnSpawn() {
        coinAnimator.SetTrigger("onSpawn");
        ScoreGain.Invoke(1);
        Jukebox.Instance.PlaySimul("coin");
        uses--;
    }

    override public void OnCollect() {
        return;
    }

    override public void OnReset() {
        coinAnimator.SetTrigger("onReset");
        uses = quantity;
    }
}
