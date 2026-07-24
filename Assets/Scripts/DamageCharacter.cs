using UnityEngine;

public static class DamageCharacter
{
    // true if dead
    public static bool Damage(in ID id, float damage, TimeAdjustmentReason reason)
    {
        Main main = Main.Singleton;
        switch (id.type)
        {
            case IDType.Player:
                if (main.isPlayerAlive &&
                    // once level is cleared, make player invulnerable in case of simulatenous shooting
                    main.levelState == LevelState.Countdown)
                {
                    bool isDead = Damage(ref main.playerCharacter.character, damage);
                    PlayerUI.Instance.GetTimer.SetRemainingTime(main.playerCharacter.character, reason);
                    return isDead;
                }
                break;
            case IDType.Enemy:
                if (main.enemyCharacters.IsValidID(id))
                {
                    ref EnemyCharacter enemy = ref main.enemyCharacters[id];
                    bool isDead = Damage(ref enemy.character, damage);
                    enemy.healthBar.SetRemainingTime(enemy.character, reason);
                    return isDead;
                }
                break;
        }
        return false;
    }

    // true if dead
    public static bool Damage(ref Character character, float damage)
    {
        character.currentHealth -= damage;
        if (character.currentHealth <= 0)
        {
            if (character.id.type == IDType.Player)
            {
                for (int i = 0; i < character.camera.childCount; i++)
                {
                    GameObject.Destroy(character.camera.GetChild(i).gameObject);
                }
                character.camera.SetParent(null);
            }
            GameObject.Destroy(character.gameObject);
            return true;
        }
        return false;
    }
}
