using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    // project constants
    private float timeDelta;
    private int fixedUpdateRate;
    // physics variables (in world units, 1 tile = 1 world unit = 16px)
    public float gravity = 50;
    public float maxSpeed = 7;
    public float accel = 0.7f;
    public float airAccel = 0.4f;
    public float accelSmoothing = 0.5f;
    public float decel = 0.4f;
    public float jumpAccel = 12;
    public float varJumpGravScale = 0.12f;
    public float varJumpDuration = 0.3f;
    // input state variables
    private float directionState = 0; // no need to Sign() this since its already unitized
    private bool jumpState = false;
    private bool jumpReleaseState = false;
    // game state variables
    private bool onGroundState = true;
    private float varJumpTimer = 0;
    private bool jumped = false;
    // physics bodies
    private Rigidbody2D marioBody;
    // sprite variables
    private SpriteRenderer marioSprite;
    // sprite state variables
    private bool faceRightState = true;

    // Start is called before the first frame update
    void Start() {
        timeDelta = Time.fixedDeltaTime;
        fixedUpdateRate = (int)(1 / Time.fixedDeltaTime);
        Debug.Log(fixedUpdateRate);
        // Set to be 30 FPS
        Application.targetFrameRate = 60;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        // We do input monitoring here since execution is guaranteed every frame
        directionState = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown("space")) {
            jumpState = true;
        }
        if (Input.GetKeyUp("space")) {
            jumpReleaseState = true;
        }
        // Sprite updates
        // toggle state
        if (Input.GetKeyDown("a") && faceRightState) {
            faceRightState = false;
            marioSprite.flipX = true;
        }
        if (Input.GetKeyDown("d") && !faceRightState) {
            faceRightState = true;
            marioSprite.flipX = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Ground")) onGroundState = true;
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
            if (!jumpReleaseState && varJumpTimer > 0) {
                // less gravity while holding jump key
                resultVelo.y += -gravity * varJumpGravScale * timeDelta;
                varJumpTimer -= timeDelta;
            }
            else {
                // back to full grav when key up or if reached max jump
                resultVelo.y += -gravity * timeDelta;
                varJumpTimer = 0;
            }
        }
        else {
            // normal gravity
            resultVelo.y += -gravity * timeDelta;
        }
        marioBody.linearVelocity = resultVelo;

        // clear jumping state flags
        jumpState = false;
        jumpReleaseState = false;
    }
}
