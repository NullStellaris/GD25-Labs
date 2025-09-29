using UnityEngine;

public abstract class ItemLogic : MonoBehaviour {
    public abstract void OnSpawn();
    public abstract void OnCollect();
    public abstract void OnReset();
    public abstract bool IsUsed();
}
