using UnityEngine;

public class BrickPowerupController : MonoBehaviour, IPowerupController
{
    public Animator powerupAnimator;
    public BasePowerup powerup; // reference to this question box's powerup

    [SerializeField] private bool breakable = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<Animator>().SetBool("breakable", breakable);
        GameManager.instance.gameRestart.AddListener(GameStart);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && !powerup.hasSpawned)
        {
            Debug.Log("Collied with" + this.name);
            // show disabled sprite
            this.GetComponent<Animator>().SetTrigger("spawned");
            // spawn the powerup
            if(powerupAnimator!=null)powerupAnimator.SetTrigger("spawned");
        }
    }

    // used by animator
    public void Disable()
    {
        if (!breakable)
        {
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    public void GameStart()
    {
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        this.GetComponent<Animator>().SetTrigger("reset");
    }
}
