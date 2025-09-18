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
    public float maxSpeed = 5;
    public float accel = 0.5f;
    public float airAccel = 0.3f;
    public float accelSmoothing = 0.8f;
    public float decel = 0.4f;
    public float jumpAccel = 12;
    public float varJumpGravScale = 0.4f;
    public float varJumpDuration = 0.5f;
    public float varJumpDeadzone = 0.2f;
    // input state variables
    private float directionState = 0; // no need to Sign() this since its already unitized
    private bool jumpState = false;
    // game state variables
    private bool onGroundState = true;
    private float varJumpTimer = 0;
    // physics bodies
    private Rigidbody2D marioBody;

    // Start is called before the first frame update
    void Start() {
        timeDelta = Time.fixedDeltaTime;
        fixedUpdateRate = (int)(1 / Time.fixedDeltaTime);
        Debug.Log(fixedUpdateRate);
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        // We do input monitoring here since execution is guaranteed every frame
        directionState = Input.GetAxisRaw("Horizontal");
        jumpState = Input.GetKeyDown("space");
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
        // apply to mario
        marioBody.linearVelocity = resultVelo;

        // jumping physics
        if (jumpState && onGroundState) {
            marioBody.AddForce(Vector2.up * jumpAccel, ForceMode2D.Impulse);
            onGroundState = false;
            varJumpTimer = varJumpDuration;
        }
        // variable jump physics and gravity
        float resultGrav = gravity;
        if (Input.GetKey("space") && !onGroundState && varJumpTimer > 0) {
            if (varJumpDuration - varJumpTimer < varJumpDeadzone) {
                Debug.Log("jump held");
                resultGrav = gravity * varJumpGravScale;
            }
            varJumpTimer = Mathf.MoveTowards(varJumpTimer, 0, timeDelta);
        }

        marioBody.AddForce(Vector2.down * resultGrav);
    }
}
