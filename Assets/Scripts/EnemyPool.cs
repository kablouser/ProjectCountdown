using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyType
    {
        public GameObject prefab;
        public int poolSize;
        public List<GameObject> pool;
    }

    public EnemyType[] enemyTypes;
    public List<Transform> enemySpawnPoints;

    private void Awake()
    {
        foreach (ref EnemyType enemyType in enemyTypes.AsSpan())
        {
            for (int i = 0; i < enemyType.poolSize; i++)
            {
                GameObject enemy = Instantiate(enemyType.prefab);
                enemy.SetActive(false);
                enemyType.pool.Add(enemy);
            }
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

    public bool GetNext(out GameObject nextEnemy, int enemyTypeI)
    {
        if (!(0 <= enemyTypeI && enemyTypeI < enemyTypes.Length))
        {
            nextEnemy = null;
            return false;
        }

        ref EnemyType enemyType = ref enemyTypes[enemyTypeI];
        for (int i = 0; i < enemyType.pool.Count; i++)
        {
            if (!enemyType.pool[i].activeInHierarchy)
            {
                enemyType.pool[i].SetActive(true);
                nextEnemy = enemyType.pool[i];
                return true;
            }
        }

        nextEnemy = null;
        return false;
    }

    public (int, int) GetActiveAndFreeCount()
    {
        int activeCount = 0;
        int poolCount = 0;
        foreach (ref EnemyType enemyType in enemyTypes.AsSpan())
        {
            for (int i = 0; i < enemyType.pool.Count; i++)
            {
                if (enemyType.pool[i].activeInHierarchy)
                {
                    activeCount++;
                }
            }
            poolCount += enemyType.pool.Count;
        }

        return (activeCount, poolCount - activeCount);
    }
}