using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    //Global variables
    public float speed = 10;
    public float maxSpeed = 20;
    private Rigidbody2D marioBody; 
    public float upSpeed = 10;
    private bool onGroundState = true; 
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    //Other variables
    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    public JumpOverGoomba jumpOverGoomba;
    public TextMeshProUGUI gameOverText;
    public RectTransform ResetButton;
    public Timer timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }

    }
    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        
        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);
            // check if it doesn't go beyond maxSpeed
            if (marioBody.linearVelocity.magnitude < maxSpeed)
                marioBody.AddForce(movement * speed);
        }

        // stop
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            // stop
            marioBody.linearVelocity = Vector2.zero;
        }

        //Jump
        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) onGroundState = true;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");
            //On death
            Time.timeScale = 0.0f;
            GameOver();
        }
    }

    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // reset position
        marioBody.transform.position = new Vector3(-4f, 1f, 0.0f);
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        scoreText.text = "Score: 0";
        jumpOverGoomba.score = 0;
        scoreText.rectTransform.localPosition = new Vector3(-700f, 480f, 0.0f);
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }
        // reset reset button
        ResetButton.localPosition = new Vector3(887f, 480f, 0.0f);
        // hide game over
        gameOverText.enabled = false;
        // reset timer
        timer.remainingTime = 90;

    }
    private void GameOver()
    {
        gameOverText.enabled = true;
        scoreText.rectTransform.localPosition = new Vector3(40f, 215f, 0.0f);
        ResetButton.localPosition = new Vector3(0.0f, 115f, 0.0f);
    }
}
    