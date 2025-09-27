using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class QuestionBox : MonoBehaviour {
    private Rigidbody2D box;
    private Vector3 origin;
    private bool sproinged = false;
    public float bonkStrength = 5;
    void Start() {
        box = GetComponent<Rigidbody2D>();
        origin = box.position;
        box.bodyType = RigidbodyType2D.Static;
        sproinged = false;
    }

    void Update() {
    }

    void FixedUpdate() {
        if (box.position.y < origin.y) {
            box.bodyType = RigidbodyType2D.Static;
            box.position = origin;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Player") && !sproinged) {
            foreach (ContactPoint2D contact in col.contacts) {
                if (contact.normal == Vector2.up) {
                    box.bodyType = RigidbodyType2D.Dynamic;
                    box.AddForce(Vector2.up * 50, ForceMode2D.Impulse);
                    col.gameObject.GetComponent<PlayerMovement>().OnBonk(bonkStrength);
                    sproinged = true;
                }
            }
        }
    }

    public void OnReset() {
        sproinged = false;
    }


}
