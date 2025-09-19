using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    private Vector3 originalPos;
    public float moveSpeed = 1.5f;
    private Vector2 velocity;
    private Rigidbody2D enemyBody;
    private bool hitWall = false;

    void Start() {
        enemyBody = GetComponent<Rigidbody2D>();
        // get starting position
        originalPos = transform.position;
    }

    void Movegoomba() {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
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

    void Update() {
        if (hitWall) {
            // change direction
            moveSpeed *= -1;
            hitWall = false;
        }
        velocity = new Vector2(moveSpeed, 0);
        Movegoomba();
    }
}