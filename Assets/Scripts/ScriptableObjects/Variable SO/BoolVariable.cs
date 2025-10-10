using UnityEngine;

[CreateAssetMenu(fileName = "New Bool Variable", menuName = "Variables/Bool")]
public class BoolVariable : ScriptableObject {
    [SerializeField] private bool value;

    public bool Value {
        get => value;
        set => this.value = value;
    }

    public void Toggle() {
        value = !value;
    }
}
