using UnityEngine;

public class EnemyCharacterProxy : MonoBehaviour, IProxy
{
    public EnemyCharacter enemyCharacter = EnemyCharacter.Default;
    public ID id;

    private void Awake()
    {
        enemyCharacter.character.id = Main.Singleton.AwakeEnemy(enemyCharacter);
    }

    private void OnDestroy()
    {
        Main.Singleton.DestroyEnemy(enemyCharacter.character.id);
    }

    ID IProxy.GetID()
    {
        return enemyCharacter.character.id;
    }
}
