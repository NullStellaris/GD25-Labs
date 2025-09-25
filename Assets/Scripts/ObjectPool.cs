using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    private int MaxPoolSzie = 20;

    private List<GameObject> pool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pool = new List<GameObject>();
        createObject();
    }

    // Update is called once per  frame
    void Update()
    {

    }

    public GameObject GetPoolObject() {
        Debug.Log(pool.Count);
        //Look for in active object
        foreach (GameObject obj in pool) {
            //Check for inactive object
            if (!obj.activeInHierarchy) {
                Debug.Log("revived object created");
                return obj;
            }
        }
        //No inactive object create object and add to pool
        if (pool.Count < MaxPoolSzie) {
            Debug.Log("new object created");
            return createObject();
        }
        return null;
    }

    private GameObject createObject() {
        GameObject obj = Instantiate(enemyPrefab, transform);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }
}
