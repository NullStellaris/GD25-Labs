using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour {
    // Input Handler
    private UserInput ReadInput;
    // project constants
    private Vector3 originalPos;
    private float timeDelta;
    private int fixedUpdateRate;
    // physics variables (in world units, 1 tile = 1 world unit = 16px)
    public float gravity = 1.1f;
    public float maxSpeed = 7;
    public float accel = 0.7f;
    public float airAccel = 0.4f;
    public float accelSmoothing = 0.5f;
    public float decel = 0.4f;
    public float jumpAccel = 16;
    public float stompAccel = 8;
    public float varJumpGravScale = 0.2f;
    public float varJumpDuration = 0.2f;
    // input state variables
    private float directionState = 0; // no need to Sign() this since its already unitized
    private bool jumpState = false;
    private bool jumpHeldState = false;
    // game state variables
    private bool onGroundState = true;
    private float varJumpTimer = 0;
    private bool jumped = false;
    // physics bodies
    private Rigidbody2D marioBody;
    // sprite variables
    private SpriteRenderer marioSprite;
    public Sprite marioDefault;
    public Sprite marioJump;
    // sprite state variables
    private bool faceRightState = true;

    // other variables
    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    public JumpOverGoomba jumpOverGoomba;
    public Canvas gameOverScreen;
    public TextMeshProUGUI gameOverText;

    void Awake() {
        ReadInput = new UserInput();
    }

    // Start is called before the first frame update
    void Start() {
        gameOverScreen.enabled = false;
        // enable input
        ReadInput.Player.Enable();
        originalPos = transform.position;
        timeDelta = Time.fixedDeltaTime;
        fixedUpdateRate = (int)(1 / Time.fixedDeltaTime);
        // Set to be 30 FPS
        Application.targetFrameRate = 60;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        // We do input monitoring here since execution is guaranteed every frame
        directionState = ReadInput.Player.Movement.ReadValue<Vector2>().x;
        if (ReadInput.Player.Jump.WasPressedThisFrame()) {
            jumpState = true;
        }
        jumpHeldState = ReadInput.Player.Jump.IsPressed();
        // Sprite updates
        // toggle state
        if (directionState == -1 && faceRightState) {
            faceRightState = false;
            marioSprite.flipX = true;
        }
        if (directionState == 1 && !faceRightState) {
            faceRightState = true;
            marioSprite.flipX = false;
        }
        if (jumped) {
            marioSprite.sprite = marioJump;
        }
        else {
            marioSprite.sprite = marioDefault;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Ground")) {
            foreach (ContactPoint2D contact in col.contacts) {
                if (contact.normal == Vector2.up) {
                    onGroundState = true;
                    jumped = false;
                }
            }
        }
    }

    public void OnDamaged() {
        gameOverScreen.enabled = true;
        gameOverText.text = "Game Over!<br><br>Score: " + jumpOverGoomba.score.ToString();
        Time.timeScale = 0.0f;
    }

    public void OnStomp() {
        marioSprite.sprite = marioJump;
        marioBody.linearVelocityY = stompAccel;
        jumpOverGoomba.score += 5;
        jumpOverGoomba.countScoreState = -1;
        jumpOverGoomba.DrawScore();
    }

    // FixedUpdate is called 50 times a second
    void FixedUpdate() {
        // process horizontal movement
        float resultAccel;
        Vector2 resultVelo = marioBody.linearVelocity;
        // set accel depending on grounded state
        resultAccel = onGroundState ? accel : airAccel;
        // read input state and calculate horizontal acceleration/force
        if (directionState != 0) {
            // if exceeding max, trail off velocity exponentially by smoothing factor
            if (Math.Abs(resultVelo.x + resultAccel * directionState) > maxSpeed) {
                resultAccel = (maxSpeed - Mathf.Abs(resultVelo.x)) * accelSmoothing;
            }
            resultVelo.x += resultAccel * directionState;
        }
        else {
            // decelerate to stop
            resultVelo.x = Mathf.MoveTowards(resultVelo.x, 0, decel);
        }

        // jumping physics
        if (jumpState && onGroundState) {
            // start jump
            resultVelo.y = jumpAccel;
            onGroundState = false;
            jumped = true;
            varJumpTimer = varJumpDuration;
        }

        if (jumped) {
            if (jumpHeldState && varJumpTimer > 0) {
                // less gravity while holding jump key
                resultVelo.y -= gravity * varJumpGravScale;
                varJumpTimer -= timeDelta;
            }
            else {
                // back to full grav when key up or if reached max jump
                resultVelo.y -= gravity;
                varJumpTimer = 0;
            }
        }
        else {
            // normal gravity
            resultVelo.y -= gravity;
        }
        marioBody.linearVelocity = resultVelo;

        // clear jumping state flags
        jumpState = false;
    }

    // other methods
    public void RestartButtonCallback(int input) {
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame() {
        // clear gameOver screen
        gameOverScreen.enabled = false;
        // cancel any momentum
        marioBody.linearVelocity = Vector2.zero;
        // reset position
        marioBody.transform.position = originalPos;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        scoreText.text = "Score: 0";
        // reset Goomba
        EnemyMovement[] enemyScripts = enemies.GetComponentsInChildren<EnemyMovement>();
        foreach (EnemyMovement enemy in enemyScripts) {
            if (enemy) {
                enemy.Reset();
            }
        }
        jumpOverGoomba.score = 0;
    }
}
