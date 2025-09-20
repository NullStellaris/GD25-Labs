using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    public Vector3 originalPos;
    public float moveSpeed = 1.5f;
    private float originalSpeed;
    public float decayTime = 1f;
    public bool alive = true;
    private Vector2 velocity;
    private Rigidbody2D enemyBody;
    private SpriteRenderer goombaSprite;
    private Vector3 originalScale;
    private bool hitWall = false;

    void Start() {
        enemyBody = GetComponent<Rigidbody2D>();
        // get starting position
        originalPos = transform.localPosition;
        goombaSprite = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        // get original movement params
        originalSpeed = moveSpeed;
    }

    public void Reset() {
        alive = true;
        transform.localScale = originalScale;
        goombaSprite.enabled = true;
        SetColliders(true);
        enemyBody.bodyType = RigidbodyType2D.Dynamic;
        enemyBody.transform.localPosition = originalPos;
        moveSpeed = originalSpeed;
    }

    void Movegoomba() {
        enemyBody.linearVelocityX = velocity.x;
    }

    void SetColliders(bool state) {
        Collider2D[] allCol = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in allCol) {
            col.enabled = state;
        }
    }

    IEnumerator GoombaStomped() {
        alive = false;
        Vector3 squishScale = originalScale;
        squishScale.y = 0.3f;
        transform.localScale = squishScale;
        enemyBody.bodyType = RigidbodyType2D.Static;
        SetColliders(false);
        yield return new WaitForSeconds(decayTime);
        goombaSprite.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (!col.gameObject.CompareTag("Player")) {
            foreach (ContactPoint2D contact in col.contacts) {
                if (contact.normal == Vector2.left || contact.normal == Vector2.right) {
                    hitWall = true;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Weapon")) {
            StartCoroutine(GoombaStomped());
        }
    }

    void Update() {
        if (hitWall) {
            // change direction
            moveSpeed *= -1;
            hitWall = false;
        }
        velocity = new Vector2(moveSpeed, 0);
        if (alive) {
            Movegoomba();
        }
    }
}