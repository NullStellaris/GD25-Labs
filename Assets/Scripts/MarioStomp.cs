using UnityEngine;

public class MarioStomp : MonoBehaviour {
    PlayerMovement player;

    void Start() {
        player = transform.parent.GetComponent<PlayerMovement>();
    }
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Enemy")) {
            player.OnStomp();
        }
    }
}
