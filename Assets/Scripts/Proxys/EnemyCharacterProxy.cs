using System;
using UnityEngine;

public class EnemyCharacterProxy : MonoBehaviour, IProxy
{
    public EnemyCharacter enemyCharacter = EnemyCharacter.Default;


    private void OnEnable()
    {
        Main main = Main.Singleton;
        enemyCharacter.character.id = main.AwakeEnemy(enemyCharacter);
        enemyCharacter.character.moveSpeed = main.levelStats.GetEnemySpeed(main.level);
    }

    private void OnDisable()
    {
        Main.Singleton.SleepEnemy(enemyCharacter.character.id);
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
