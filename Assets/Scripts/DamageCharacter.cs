using UnityEngine;

public static class DamageCharacter
{
    public static bool Damage(in ID id, float damage)
    {
        Main main = Main.Singleton;
        switch (id.type)
        {
            case IDType.Player:
                if (main.isPlayerValid)
                {
                    Damage(ref main.playerCharacter.character, damage);
                    PlayerUI.Instance.GetTimer.SetRemainingTime(main.playerCharacter.character, TimeAdjustmentReason.DAMAGE);
                    return true;
                }
                break;
            case IDType.Enemy:
                if (main.enemyCharacters.IsValidID(id))
                {
                    ref EnemyCharacter enemy = ref main.enemyCharacters[id];
                    Damage(ref enemy.character, damage);
                    enemy.healthBar.SetRemainingTime(enemy.character, TimeAdjustmentReason.DAMAGE);
                    return true;
                }
                break;
        }
        return false;
    }

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
