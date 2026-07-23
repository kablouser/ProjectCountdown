using UnityEngine;

public class EnemyCharacterProxy : MonoBehaviour
{
    public EnemyCharacter enemyCharacter = EnemyCharacter.Default;

    private void Awake()
    {
        Main.Singleton.RegisterEnemy(enemyCharacter);
    }
}
