using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    //Constant variables
    public GameConstants gameConstants;
    float speed;
    float maxSpeed;
    float upSpeed;
    float deathImpulse;

    //Private
    private Rigidbody2D marioBody;
    private bool onGroundState = true;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);
    private bool moving = false;
    private bool jumpedState = false;
    private Vector3 marioInitalPos;
    private Vector3 cameraInitalPos;

    // Public vars
    [System.NonSerialized]
    public bool alive = true;
    public bool canMove;


    //Other variables
    public Timer timer;
    public Transform gameCamera;
    public Animator marioAnimator;
    public AudioClip jump;
    public AudioClip die;
    void Awake()
    {
        // other instructions
        // subscribe to Game Restart event
        GameManager.instance.gameRestart.AddListener(GameRestart);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set constants
        speed = gameConstants.speed;
        maxSpeed = gameConstants.maxSpeed;
        deathImpulse = gameConstants.deathImpulse;
        upSpeed = gameConstants.upSpeed;
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);
        marioInitalPos = this.transform.position;
        cameraInitalPos = gameCamera.position;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(alive)
        {
            marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
        }
    }
    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }

    //Movement
    public void JumpHold()
    {
        if (alive && jumpedState && canMove)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;

        }
    }
    public void Jump()
    {
        if (alive && onGroundState && canMove)
        {
            // jump
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);

        }
    }
    void Move(int value)
    {
        Vector2 movement = new Vector2(value, 0);
        // check if it doesn't go beyond maxSpeed
        if (marioBody.linearVelocity.magnitude < maxSpeed && canMove)
            marioBody.AddForce(movement * speed);
    }
    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }
    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState && canMove)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState && canMove)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) & !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && alive)
        {
            //On death play death animation
            marioBody.linearVelocity = Vector2.zero;
            marioAnimator.Play("Mario-die"); 
            SoundManager.PlaysoundFXClip(die, transform.position, 1f);
            alive = false;
        }
    }

    public void GameRestart()
    {
        // reset position
        marioBody.transform.position = marioInitalPos;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        // reset camera position
        gameCamera.position = cameraInitalPos;

        //Reset power
        Transform magePower = gameObject.transform.Find("mage");
        magePower.gameObject.SetActive(false);
    }

    // Used by animator
    void PlayJumpSound()
    {
        // play jump sound
        SoundManager.PlaysoundFXClip(jump, transform.position, 1f);
    }
    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }
    void GameOverScene()
    {
        // stop time
        Time.timeScale = 0.0f;
        // set gameover scene
        GameManager.instance.GameOver();
    }
}
