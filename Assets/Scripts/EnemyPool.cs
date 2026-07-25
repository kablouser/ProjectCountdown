using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int poolSize = 10;

    public List<Transform> enemySpawnPoints;

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

        Main.Singleton.enemyPool = this;
    }

    private void OnDrawGizmos()
    {
        if (enemySpawnPoints != null)
        {
            foreach (Transform spawnPoint in enemySpawnPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(spawnPoint.position, 0.5f);

                Gizmos.color = Color.blue;
                // Gizmos.DrawRay(transform.position, transform.forward * 2f);

                GizmosMore.DrawArrow(spawnPoint.position, spawnPoint.position + (spawnPoint.forward * 2f));

                Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
                Gizmos.DrawWireSphere(spawnPoint.position, 1f);
            }
        }
    }

    public GameObject GetNext()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                return pool[i];
            }
        }
             
        return null;
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