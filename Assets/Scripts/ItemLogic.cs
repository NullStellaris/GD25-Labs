using Unity.Properties;
using UnityEngine;

public abstract class ItemLogic : MonoBehaviour {
    [SerializeField] protected int quantity;
    protected int uses;
    public abstract void OnSpawn();
    public abstract void OnCollect();
    public abstract void OnReset();
    public int GetMaxUses() {
        return quantity;
    }
    public int GetUses() {
        return uses;
    }
}
