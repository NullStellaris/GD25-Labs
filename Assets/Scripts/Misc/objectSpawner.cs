using UnityEngine;
using System.Collections;

public class objectSpawner : MonoBehaviour
{
    public objectPool enemyPool;
    public float spawnInterval = 5f;
    //Object's script need a reset method to call when they are enabled
    private EnemyMovement enemyInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnObject();
        }
    }
    void SpawnObject()
    {
        GameObject enemy = enemyPool.GetPoolObject();
        if (enemy != null)
        {
            enemy.SetActive(true);
            //Need to access script that has the reset method how to make modular?
            enemyInstance = enemy.GetComponent<EnemyMovement>();
            enemyInstance.Reset();
            enemy.transform.position = transform.position;
        }
    }
}
