using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoints : MonoBehaviour
{
    public List<Transform> enemySpawnPoints;

    private void Awake()
    {
        Main.Singleton.enemySpawnPoints = this;
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
}
