using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class CoinLogic : ItemLogic {
    public Animator coinAnimator;
    void Start() {
        uses = quantity;
    }

    override public void OnSpawn() {
        coinAnimator.SetTrigger("onSpawn");
        GameManager.Instance.OnScore(1);
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
