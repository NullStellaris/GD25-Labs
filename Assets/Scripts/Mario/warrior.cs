using UnityEngine;

public class warrior : MonoBehaviour
{
    private Animator animator;
    private Transform attackPoint;
    public  float attackRad = 0.5f;
    public LayerMask enemies;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            swing();
        }
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackPoint = transform.GetChild(0).GetComponent<Transform>();
    }

    void Attack()
    {
        //Deteck enemy
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRad, enemies);
        foreach(Collider2D enemy in hitEnemies)
        {
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                //call method instead of using collider so can pass things like dmg or effects
                enemyMovement.death();
            }
        }
    }
    
    void swing()
    {
        animator.Play("Hammer attack");
    }
}
