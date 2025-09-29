using UnityEngine;
using UnityEngine.Rendering;

public class CoinLogic : ItemLogic {
    public Animator coinAnimator;
    public int quantity = 1;
    private int uses;

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

    public override bool IsUsed() {
        return uses <= 0;
    }
}
