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
using UnityEngine.InputSystem;

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
    [NonSerialized] public float directionState = 0; // no need to Sign() this since its already unitized
    [NonSerialized] public bool inJump = false;
    [NonSerialized] public bool jumpHeldState = false;
    [NonSerialized] public bool sprintState = false;
    // game state variables
    [NonSerialized] public bool onGroundState = true;
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

    // events
    public UnityEvent TookDamage;

    void Awake() {
        ReadInput = new UserInput();

        ReadInput.Player.Movement.performed += OnMove;
        ReadInput.Player.Movement.canceled += OnMoveCanceled;

        ReadInput.Player.Sprint.started += OnSprintStarted;
        ReadInput.Player.Sprint.canceled += OnSprintCanceled;

        ReadInput.Player.Jump.started += OnJumpStarted;
        ReadInput.Player.Jump.canceled += OnJumpCanceled;
    }

    private void UpdateSpriteDir() {
        if (alive) {
            if (directionState < 0 && faceRightState) {
                faceRightState = false;
                marioSprite.flipX = true;
            }
            else if (directionState > 0 && !faceRightState) {
                faceRightState = true;
                marioSprite.flipX = false;
            }
        }
    }

    private void OnMove(InputAction.CallbackContext context) {
        directionState = context.ReadValue<Vector2>().x;
        UpdateSpriteDir();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context) {
        directionState = 0;
    }

    private void OnSprintStarted(InputAction.CallbackContext context) {
        sprintState = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context) {
        sprintState = false;
    }

    private void OnJumpStarted(InputAction.CallbackContext context) {
        jumpHeldState = true;
        jumped = true; // lock that is only released by touching ground
    }

    private void OnJumpCanceled(InputAction.CallbackContext context) {
        jumpHeldState = false;
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
        marioAnimator.SetBool("onJump", inJump || stomped);
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
                    inJump = false;
                    stomped = false;
                }
            }
        }
    }

    public bool OnGroundCheck() {
        return (bool)Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, stepMask);
    }

    [ContextMenu("Kill Player")]
    public void Damaged() {
        alive = false;
        marioAnimator.SetTrigger("onDeath");
        GameManager.Instance.OnPlayerDeath();
    }

    public void Stomp() {
        stomped = true;
        marioBody.linearVelocityY = stompAccel;
        Jukebox.Instance.PlaySimul("stomp");
        GameManager.Instance.OnScore(2);
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
                if (Math.Abs(resultVelo.x + resultAccel * directionState) >= maxSpeed * (sprintState ? sprintMul : 1)) {
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
        if (jumped && onGroundState && OnGroundCheck()) {
            // start jump
            resultVelo.y = jumpAccel * (Mathf.Abs(marioBody.linearVelocityX) > maxSpeed ? sprintMul : 1);
            onGroundState = false;
            inJump = true;
            varJumpTimer = varJumpDuration;
            Jukebox.Instance.PlaySimul("jump");
        }

        if (inJump) {
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
        jumped = false;
    }

    public void OnReset() {
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
