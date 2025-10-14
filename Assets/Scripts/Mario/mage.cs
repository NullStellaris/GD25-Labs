using UnityEngine;

public class mage : MonoBehaviour
{
    public Transform attackPoint;
    public GameObject fireboltPrefab;

    private Vector2 aimDirection = Vector2.right;
    [SerializeField]
    private float cd = 0.5f;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = cd;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.E) && timer<0)
        {
            HandleAiming();
            Shoot();
        }
    }

    public void Shoot()
    {
        //replace with pool call
        GameObject firebolt = Instantiate(fireboltPrefab, attackPoint.position, Quaternion.identity);
        firebolt.GetComponent<Fireball>().direction = aimDirection;
        timer = cd;
    }

    private void HandleAiming()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPos - transform.position;
        aimDirection = direction.normalized;//Unit vector
    }

}
