using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    // Input Handler
    private UserInput ReadInput;
    // project constants
    private Vector3 originalPos;
    private float timeDelta;
    private int fixedUpdateRate;
    // physics variables (in world units, 1 tile = 1 world unit = 16px)
    public float gravity = 1.1f;
    public float maxSpeed = 6.5f;
    public float accel = 0.4f;
    public float airAccel = 0.4f;
    public float accelSmoothing = 0.5f;
    public float decel = 0.4f;
    public float skidDecel = 2.0f;
    public float skidThreshold = 0.95f;
    public float sprintMul = 1.1f;
    public float jumpAccel = 16;
    public float stompAccel = 8;
    public float varJumpGravScale = 0.2f;
    public float varJumpDuration = 0.2f;
    // physics checking variables
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;
    // input state variables
    private float directionState = 0; // no need to Sign() this since its already unitized
    private bool jumpState = false;
    private bool jumpHeldState = false;
    private bool sprintState = false;
    // game state variables
    private bool onGroundState = true;
    private float varJumpTimer = 0;
    private bool skidding = false;
    private bool jumped = false;
    private bool stomped = false;
    private bool alive = true;
    // physics bodies
    private Rigidbody2D marioBody;
    // sprite variables
    public SpriteRenderer marioSprite;
    // sprite state variables
    private bool faceRightState = true;

    // other variables
    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    public JumpOverGoomba jumpOverGoomba;
    public Canvas gameOverScreen;
    public TextMeshProUGUI gameOverText;
    private Jukebox jukebox;

    // animation variables
    public Animator marioAnimator;

    void Awake() {
        ReadInput = new UserInput();
    }

    // Start is called before the first frame update
    void Start() {
        jukebox = GetComponent<Jukebox>();
        jukebox.PlayOver("level", true);
        alive = true;
        gameOverScreen.enabled = false;
        // enable input
        ReadInput.Player.Enable();
        originalPos = transform.position;
        timeDelta = Time.fixedDeltaTime;
        fixedUpdateRate = (int)(1 / Time.fixedDeltaTime);
        // Set to be 30 FPS
        Application.targetFrameRate = 60;
        marioBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        // We do input monitoring here since execution is guaranteed every frame
        directionState = ReadInput.Player.Movement.ReadValue<Vector2>().x;
        sprintState = ReadInput.Player.Sprint.IsPressed();
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
        marioAnimator.SetBool("onJump", jumped || stomped);
        marioAnimator.SetBool("onDeath", !alive);
        marioAnimator.SetBool("onSkid", skidding);
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocityX));
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }

    void SetButtonsInteractable(bool state) {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons) {
            button.interactable = state;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Ground")) {
            foreach (ContactPoint2D contact in col.contacts) {
                if (contact.normal == Vector2.up) {
                    onGroundState = true;
                    jumped = false;
                    stomped = false;
                }
            }
        }
    }

    public bool OnGroundCheck() {
        return (bool)Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, layerMask);
    }

    IEnumerator GameOver() {
        Time.timeScale = 0.0f;
        alive = false;
        SetButtonsInteractable(false);
        jukebox.PlayOver("dead", false);
        yield return new WaitForSecondsRealtime(3.5f);
        gameOverScreen.enabled = true;
        SetButtonsInteractable(true);
        gameOverText.text = "Game Over!<br><br>Score: " + jumpOverGoomba.score.ToString();
    }

    public void OnDamaged() {
        StartCoroutine(GameOver());
    }

    public void OnStomp() {
        stomped = true;
        marioBody.linearVelocityY = stompAccel;
        jumpOverGoomba.score += 5;
        jumpOverGoomba.countScoreState = -1;
        jumpOverGoomba.DrawScore();
        jukebox.PlaySimul("stomp", false);
    }

    // FixedUpdate is called 50 times a second
    void FixedUpdate() {
        // process horizontal movement
        float resultAccel;
        Vector2 resultVelo = marioBody.linearVelocity;
        // set accel depending on state
        resultAccel = !onGroundState ? airAccel : accel;
        // read input state and calculate horizontal acceleration/force
        if (directionState != 0) {
            if (Mathf.Sign(marioBody.linearVelocityX) != directionState && Mathf.Abs(marioBody.linearVelocityX) > skidThreshold * maxSpeed && OnGroundCheck()) {
                jukebox.PlaySimul("twirl", false);
                skidding = true;
                resultAccel = skidDecel;
            }
            if (!skidding || (skidding && Mathf.Abs(marioBody.linearVelocityX) < 1)) {
                resultAccel *= sprintState ? sprintMul : 1;
                skidding = false;
                // if exceeding max, trail off velocity exponentially by smoothing factor
                if (Math.Abs(resultVelo.x + resultAccel * directionState) > maxSpeed * (sprintState ? sprintMul : 1)) {
                    resultAccel = (maxSpeed - Mathf.Abs(resultVelo.x)) * accelSmoothing;
                }
            }
            resultVelo.x += resultAccel * directionState;
        }
        else {
            // decelerate to stop
            resultVelo.x = Mathf.MoveTowards(resultVelo.x, 0, decel);
        }

        // jumping physics
        if (jumpState && onGroundState && OnGroundCheck()) {
            // start jump
            resultVelo.y = jumpAccel * (Mathf.Abs(marioBody.linearVelocityX) > maxSpeed ? sprintMul : 1);
            onGroundState = false;
            jumped = true;
            varJumpTimer = varJumpDuration;
            jukebox.PlaySimul("jump", false);
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
        // resurrect mario with necromancy
        alive = true;
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
        // reset level audio
        jukebox.PlayOver("level", false);
    }
}
