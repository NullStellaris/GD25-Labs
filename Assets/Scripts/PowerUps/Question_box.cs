using UnityEngine;

public class Question_box : MonoBehaviour
{

    private SpriteRenderer BoxSprite;
    [SerializeField] private float blinkSpeed = 0.2f;
    private Sprite OriginalSprite;
    private bool changed = false;
    private float timer = 0;//tracks the time it has been in a state
    //For disabling
    private bool disabled;
    private Rigidbody2D rigidbody2D;
    private SpringJoint2D SpringJoint2D;

    public CoinAnimation CoinAnimation;
    public Sprite NewSprite;
    public Sprite DisabledSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BoxSprite = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        SpringJoint2D = GetComponent<SpringJoint2D>();
        OriginalSprite = BoxSprite.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (!disabled)
        {
            timer += Time.deltaTime;
            if (timer > blinkSpeed) changeSprite();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {   
        if(col.gameObject.name == "Mario" && !disabled)
        {
            //when hit by mario
            CoinAnimation.Jump();
            BoxSprite.sprite = DisabledSprite;
            disabled = true;
            Invoke(nameof(afterJump), 1f);
        }
    }
    void afterJump()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        SpringJoint2D.enabled = false;
    }
    void changeSprite()
    {
        timer = 0;
        if (changed)
        {
            BoxSprite.sprite = OriginalSprite;
            changed = false;
        }
        else
        {
            BoxSprite.sprite = NewSprite;
            changed = true;
        }
    }
}
