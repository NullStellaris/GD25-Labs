using System.Collections.Generic;
using UnityEngine;

public class objectPool : MonoBehaviour
{
    //Prefab to spwan
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private int MaxPoolSzie = 20;
    private List<GameObject> pool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pool = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //One of 3 things will happen 1) reuse 2) create 3) maximum reached
    public GameObject GetPoolObject()
    {
        //Look for in active object in pool only not parent object
        //So you can have many objects in the parent but pool only affects itself
        //Always retun unactivated
        foreach (GameObject obj in pool)
        {
            //Check for inactive object
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        //No inactive object create object and add to pool
        if (pool.Count < MaxPoolSzie)
        {
            return createObject();
        }
        //Limit reached
        return null;
    }
    private GameObject createObject()
    {
        GameObject obj = Instantiate(enemyPrefab, transform);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public void GameRestart()
    {
        foreach (GameObject obj in pool)
        {
            Destroy(obj);
        }
        pool.Clear();
    }
}
