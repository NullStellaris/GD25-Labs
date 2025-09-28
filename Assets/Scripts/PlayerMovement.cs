using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour {
    // Input Handler
    private UserInput ReadInput;
    // project constants
    private Vector3 originalPos;
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
    private LayerMask stepMask;
    // input state variables
    [System.NonSerialized] public float directionState = 0; // no need to Sign() this since its already unitized
    [System.NonSerialized] public bool jumpState = false;
    [System.NonSerialized] public bool inJump = false;
    [System.NonSerialized] public bool jumpHeldState = false;
    [System.NonSerialized] public bool sprintState = false;
    // game state variables
    [System.NonSerialized] public bool onGroundState = true;
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

    // animation variables
    public Animator marioAnimator;

    void Awake() {
        ReadInput = new UserInput();
    }

    // Start is called before the first frame update
    void Start() {
        alive = true;
        // enable input
        ReadInput.Player.Enable();
        originalPos = transform.position;
        // Set to be 30 FPS
        Application.targetFrameRate = 60;
        marioBody = GetComponent<Rigidbody2D>();
        // Get layers of interest
        int groundLayer = 1 << LayerMask.NameToLayer("Ground");
        int obstacleLayer = 1 << LayerMask.NameToLayer("Obstacles");
        stepMask = groundLayer | obstacleLayer;
    }

    // Update is called once per frame
    void Update() {
        // We do input monitoring here since execution is guaranteed every frame
        directionState = ReadInput.Player.Movement.ReadValue<Vector2>().x;
        sprintState = ReadInput.Player.Sprint.IsPressed();
        jumpState = ReadInput.Player.Jump.WasPressedThisFrame();
        jumpHeldState = ReadInput.Player.Jump.IsPressed();
        if (jumpState) {
            inJump = true;
        }
        // Sprite updates
        // toggle state
        if (directionState == -1 && faceRightState && alive) {
            faceRightState = false;
            marioSprite.flipX = true;
        }
        if (directionState == 1 && !faceRightState && alive) {
            faceRightState = true;
            marioSprite.flipX = false;
        }
        marioAnimator.SetBool("onJump", jumped || stomped);
        marioAnimator.SetBool("onSkid", skidding);
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocityX));
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Obstacle")) {
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
        return (bool)Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, stepMask);
    }

    public UnityEvent onDamaged;
    [ContextMenu("Kill Player")]
    public void Damaged() {
        alive = false;
        marioAnimator.SetTrigger("onDeath");
        onDamaged.Invoke();
    }

    public UnityEvent<int> onScore;
    public void Stomp() {
        stomped = true;
        marioBody.linearVelocityY = stompAccel;
        Jukebox.Instance.PlaySimul("stomp");
        onScore.Invoke(5);
    }

    public void Bonk(float force) {
        marioBody.linearVelocityY = -force;
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
                Jukebox.Instance.PlaySimul("twirl");
                skidding = true;
                resultAccel = skidDecel;
            }
            if (!skidding || (skidding && Mathf.Abs(marioBody.linearVelocityX) < 1)) {
                resultAccel *= sprintState ? sprintMul : 1;
                skidding = false;
                // if exceeding max, trail off velocity exponentially by smoothing factor
                if (Math.Abs(resultVelo.x + resultAccel * directionState) > maxSpeed * (sprintState ? sprintMul : 1)) {
                    resultAccel = ((maxSpeed * (sprintState ? sprintMul : 1)) - Mathf.Abs(resultVelo.x)) * accelSmoothing;
                }
            }
            resultVelo.x += resultAccel * directionState;
        }
        else {
            // decelerate to stop
            resultVelo.x = Mathf.MoveTowards(resultVelo.x, 0, decel);
        }

        // jumping physics
        if (inJump && onGroundState && OnGroundCheck()) {
            // start jump
            resultVelo.y = jumpAccel * (Mathf.Abs(marioBody.linearVelocityX) > maxSpeed ? sprintMul : 1);
            onGroundState = false;
            jumped = true;
            varJumpTimer = varJumpDuration;
            Jukebox.Instance.PlaySimul("jump");
        }

        if (jumped) {
            if (jumpHeldState && varJumpTimer > 0) {
                // less gravity while holding jump key
                resultVelo.y -= gravity * varJumpGravScale;
                varJumpTimer -= Time.fixedDeltaTime;
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
        inJump = false;
    }

    void OnEnable() {
        GameManager.GlobalReset += OnReset;
    }

    void OnDisable() {
        GameManager.GlobalReset -= OnReset;
    }

    void OnReset() {
        // resurrect mario with necromancy
        alive = true;
        marioAnimator.SetTrigger("onReset");
        // cancel any momentum
        marioBody.linearVelocity = Vector2.zero;
        // reset position
        marioBody.transform.position = originalPos;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
    }
}
