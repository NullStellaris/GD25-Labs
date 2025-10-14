using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public objectPool enemyPool;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.gameRestart.AddListener(GameRestart);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameRestart()
    {
        foreach (Transform child in transform)
        {
            enemyPool.GameRestart();
            child.gameObject.SetActive(true);
            child.GetComponent<EnemyMovement>().Reset();
            child.gameObject.SetActive(true);
        }
    }
}
