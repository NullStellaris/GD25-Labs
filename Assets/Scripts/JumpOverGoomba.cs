using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class JumpOverGoomba : MonoBehaviour {
    public GameObject enemies;
    public TextMeshProUGUI scoreText;
    private bool onGroundState;

    [System.NonSerialized]
    public int score = 0; // we don't want this to show up in the inspector

    public int countScoreState = -1;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;

    // Input Processor
    private UserInput ReadInput;
    // Input State Variables
    private bool jumpState = false;

    void Awake() {
        ReadInput = new UserInput();
    }

    // Start is called before the first frame update
    void Start() {
        ReadInput.Player.Enable();
    }

    // Update is called once per frame
    void Update() {
        if (ReadInput.Player.Jump.WasPressedThisFrame()) {
            jumpState = true;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }

    public void DrawScore() {
        scoreText.text = "Score: " + score.ToString();
    }

    void FixedUpdate() {
        // mario jumps
        if (jumpState && onGroundCheck()) {
            onGroundState = false;
            countScoreState = 0;
        }

        // when jumping, and Goomba is near Mario and we haven't registered our score
        if (!onGroundState && countScoreState == 0) {
            Transform[] enemyLocations = enemies.GetComponentsInChildren<Transform>();
            foreach (Transform enemyLocation in enemyLocations) {
                if (Mathf.Abs(transform.position.x - enemyLocation.position.x) < 0.5f && enemyLocation.gameObject.CompareTag("Enemy") && enemyLocation.position.y < transform.position.y) {
                    countScoreState = 1;
                }
            }
        }

        jumpState = false;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Ground")) {
            onGroundState = true;
            if (countScoreState == 1) {
                score++;
                DrawScore();
            }
            countScoreState = -1;
        }
    }


    private bool onGroundCheck() {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, layerMask)) {
            return true;
        }
        else {
            return false;
        }
    }
}
