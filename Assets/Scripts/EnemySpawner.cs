using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public ObjectPool enemyPool;
    public float spawnInterval = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    IEnumerator SpawnEnemyCoroutine() {
        while (true) {
            yield return new WaitForSeconds(spawnInterval);

            SpawnObject();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    void SpawnObject() {
        GameObject enemy = enemyPool.GetPoolObject();
        if (enemy != null) {
            enemy.transform.position = transform.position;
            enemy.SetActive(true);
        }
    }
}
