using System;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        Main.Singleton.enemySpawnPoints.Add(this);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);

        Gizmos.color = Color.blue;
        // Gizmos.DrawRay(transform.position, transform.forward * 2f);
        
        GizmosMore.DrawArrow(transform.position, transform.position + (transform.forward * 2f));

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}