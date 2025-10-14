using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushroomPowerup : BasePowerup
{
    // setup this object's type
    // instantiate variables
    protected BoxCollider2D m_Collider;
    public AudioClip powerup_appears;

    protected override void Start()
    {
        base.Start(); // call base class Start()
        this.type = PowerupType.MagicMushroom;
        rigidBody.bodyType = RigidbodyType2D.Static;
        m_Collider = GetComponent<BoxCollider2D>();
        m_Collider.enabled = false;
        GameManager.instance.gameRestart.AddListener(GameStart); 
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && spawned)
        {
            // TODO: do something when colliding with Player
            Transform playerChild = col.transform.Find("mage");
            playerChild.gameObject.SetActive(true);

            // then destroy powerup (optional)
            gameObject.SetActive(false);

        }
        else if (col.gameObject.layer >= 6) // else if hitting Pipe, flip travel direction
        {
            if (spawned)
            {
                goRight = !goRight;
                rigidBody.AddForce(Vector2.right * 3 * (goRight ? 1 : -1), ForceMode2D.Impulse);
            }
        }
    }
    // interface implementation
    public override void SpawnPowerup()
    {
        SoundManager.PlaysoundFXClip(powerup_appears, transform.position, 1f);
        spawned = true;
        m_Collider.enabled = true;
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(spawnMushroom());
    }
    private IEnumerator spawnMushroom()
    {
        yield return new WaitForFixedUpdate();
        rigidBody.AddForce(Vector2.right * 3f, ForceMode2D.Impulse);
    }

    // interface implementation
    public override void ApplyPowerup(MonoBehaviour i)
    {
        // TODO: do something with the object

    }
    public void GameStart()
    {
        gameObject.SetActive(true);
        this.transform.localPosition = new Vector3(0, 1);
        m_Collider.enabled = false;
        rigidBody.bodyType = RigidbodyType2D.Static;
        spawned = false;
        this.GetComponentInChildren<Animator>().SetTrigger("reset");
    }
}