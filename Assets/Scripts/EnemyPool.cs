using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int poolSize = 10;
    
    List<GameObject> pool;

    private void Awake()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            pool.Add(enemy);
        }
    }

    public bool GetNext(out GameObject nextEnemy)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                nextEnemy = pool[i];
                return true;
            }
        }

        nextEnemy = null;     
        return false;
    }

    public (int, int) GetActiveAndFreeCount()
    {
        int activeCount = 0;
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].activeInHierarchy)
            {
                activeCount++;
            }
        }
        
        return (activeCount, pool.Count - activeCount);
    }
}