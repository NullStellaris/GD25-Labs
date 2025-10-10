using UnityEngine;

[CreateAssetMenu(fileName = "New Int Variable", menuName = "Variables/Int")]
public class IntVariable : ScriptableObject {
    [SerializeField] private int value;

    public int Value {
        get => value;
        set => this.value = value;
    }

    public void SetValue(int newValue) {
        value = newValue;
    }

    public void Add(int amount) {
        value += amount;
    }
}
