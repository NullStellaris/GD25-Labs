using UnityEngine;
using UnityEngine.UIElements;

public class Fireball : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 direction = Vector2.right;
    public float lifespan = 2.0f;
    public float speed;
    public LayerMask enemies;
    public AudioClip fire;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * speed;
        Rotate();
        Destroy(gameObject, lifespan); 
        SoundManager.PlaysoundFXClip(fire, transform.position, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void Rotate()
    {
        float angel = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angel));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemies) != 0)
        {
            //Debug.Log("Hit" + collision.name);
            EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.death();
                Destroy(gameObject);
            }
        }
    }
}
