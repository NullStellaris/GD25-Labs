using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class QuestionBox : MonoBehaviour {
    private Rigidbody2D box;
    private Vector3 origin;
    public float bonkStrength = 5;

    public Animator questionAnimator;
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
        if (col.gameObject.CompareTag("Player") && prize.GetUses() > 0) {
            foreach (ContactPoint2D contact in col.contacts) {
                if (contact.normal == Vector2.up) {
                    PlayerMovement player = col.gameObject.GetComponent<PlayerMovement>();
                    box.bodyType = RigidbodyType2D.Dynamic;
                    box.AddForce(Vector2.up * 50, ForceMode2D.Impulse);
                    player.Bonk(bonkStrength);
                    GameManager.Instance.mixer.SetFloat("Pitch", 1.0f);
                    prize.OnSpawn();
                    if (prize.GetUses() <= 0) {
                        questionAnimator.SetTrigger("onSproing");
                    }
                    return;
                }
            }
        }
    }


    public void OnReset() {
        prize.OnReset();
        questionAnimator.SetTrigger("onReset");
    }
}
