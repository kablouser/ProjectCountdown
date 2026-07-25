using System;
using UnityEngine;

public class EnemyCharacterProxy : MonoBehaviour, IProxy
{
    public EnemyCharacter enemyCharacter = EnemyCharacter.Default;
    public ID id;


    private void OnEnable()
    {
        enemyCharacter.character.id = Main.Singleton.AwakeEnemy(enemyCharacter);
    }

    private void OnDisable()
    {
        Main.Singleton.SleepEnemy(id);
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
