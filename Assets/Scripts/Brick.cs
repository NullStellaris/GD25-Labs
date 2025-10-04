using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour {
    private Rigidbody2D box;
    private Vector3 origin;
    public float bonkStrength = 5;
    public GameObject prizeContainer;
    private ItemLogic prize;


    // prefab, so lets subscribe to reset here
    void Start() {
        box = GetComponent<Rigidbody2D>();
        origin = box.position;
        box.bodyType = RigidbodyType2D.Static;
        prize = prizeContainer.GetComponentInChildren<ItemLogic>();
        GameManager.Instance.GlobalReset.AddListener(OnReset);
    }

    void OnDisable() {
        GameManager.Instance.GlobalReset.RemoveListener(OnReset);
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
                    GameManager.Instance.mixer.SetFloat("Pitch", 1.0f);
                    if (prize != null && prize.GetUses() > 0) {
                        prize.OnSpawn();
                        UpdatePitch();
                    }
                    return;
                }
            }
        }
    }

    void UpdatePitch() {
        float shiftSemitones = 3.0f * (prize.GetMaxUses() - prize.GetUses() - 1) / (prize.GetMaxUses() - 1);
        float shiftProportion = Mathf.Pow(2.0f, shiftSemitones / 12.0f);
        GameManager.Instance.mixer.SetFloat("Pitch", shiftProportion);
    }

    public void OnReset() {
        GameManager.Instance.mixer.SetFloat("Pitch", 1.0f);
        if (prize != null) {
            prize.OnReset();
        }
    }
}
