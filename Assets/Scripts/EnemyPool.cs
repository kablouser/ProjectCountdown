using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    private void Awake()
    {
        AllocatePool();
    }

    [ContextMenu("Allocate Pool")]
    private void AllocatePool()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Undo.RecordObject(this, "Allocate EnemyPool");
        }
#endif

        foreach (ref EnemyType enemyType in enemyTypes.AsSpan())
        {
            for (int i = enemyType.pool.Count; i < enemyType.poolSize; i++)
            {
                GameObject enemy = Instantiate(enemyType.prefab, transform);
                enemy.SetActive(false);
                enemyType.pool.Add(enemy);

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    Undo.RegisterCreatedObjectUndo(enemy, "Allocate EnemyPool");
                }
#endif
            }

#if UNITY_EDITOR // remove excess
            if (!Application.isPlaying &&
                enemyType.poolSize < enemyType.pool.Count)
            {
                for (int i = enemyType.poolSize; i < enemyType.pool.Count; i++)
                {
                    if (enemyType.pool[i] != null)
                    {
                        Undo.DestroyObjectImmediate(enemyType.pool[i]);
                    }
                }
                enemyType.pool.RemoveRange(enemyType.poolSize, enemyType.pool.Count - enemyType.poolSize);
            }
#endif
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