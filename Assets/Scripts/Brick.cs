using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Brick : MonoBehaviour {
    private Rigidbody2D box;
    private Vector3 origin;
    public float bonkStrength = 5;
    public GameObject prizeContainer;
    private ItemLogic prize;
    void Start() {
        box = GetComponent<Rigidbody2D>();
        origin = box.position;
        box.bodyType = RigidbodyType2D.Static;
        prize = prizeContainer.GetComponentInChildren<ItemLogic>();
    }

    void FixedUpdate() {
        if (box.position.y < origin.y) {
            box.bodyType = RigidbodyType2D.Static;
            box.position = origin;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Player")) {
            foreach (ContactPoint2D contact in col.contacts) {
                if (contact.normal == Vector2.up) {
                    PlayerMovement player = col.gameObject.GetComponent<PlayerMovement>();
                    box.bodyType = RigidbodyType2D.Dynamic;
                    box.AddForce(Vector2.up * 50, ForceMode2D.Impulse);
                    player.Bonk(bonkStrength);
                    if (prize != null && !prize.IsUsed()) {
                        prize.OnSpawn();
                    }
                    return;
                }
            }
        }
    }

    void OnEnable() {
        GameManager.GlobalReset += OnReset;
    }

    void OnDisable() {
        GameManager.GlobalReset -= OnReset;
    }

    public void OnReset() {
        if (prize != null) {
            prize.OnReset();
        }
    }
}
