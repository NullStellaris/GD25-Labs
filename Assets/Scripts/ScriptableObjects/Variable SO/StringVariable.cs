using UnityEngine;

[CreateAssetMenu(fileName = "New String Variable", menuName = "Variables/String")]
public class StringVariable : ScriptableObject {
    [SerializeField] private string value;
    public string Value {
        get => value;
        set => this.value = value;
    }
}
