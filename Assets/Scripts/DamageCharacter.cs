using UnityEditor.Build.Content;
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
                if (main.isPlayerAlive)
                {
                    // once level is cleared, make player invulnerable in case of simulatenous shooting
                    if (main.levelState == LevelState.LevelCleared && 0f < damage)
                    {
                        return false;
                    }
                    // allow healing to go through

                    bool isDead = InternalDamage(ref main.playerCharacter.character, damage);
                    PlayerUI.Instance.healthBar.SetRemainingTime(main.playerCharacter.character, reason);
                    return isDead;
                }
                break;
            case IDType.Enemy:
                if (main.enemyCharacters.IsValidID(id))
                {
                    ref EnemyCharacter enemy = ref main.enemyCharacters[id];
                    bool isDead = InternalDamage(ref enemy.character, damage);
                    if (!isDead)
                    {
                        enemy.healthBar.SetRemainingTime(enemy.character, reason);
                    }

                    PlayerUI.Instance.ShowHitMarker(); 

                    if (isDead)
                    {
                        if (Random.value <= enemy.dropExtraTimeChance)
                        {
                            PickUp pickup = GameObject.Instantiate(main.pickUpExtraTimePrefab, enemy.character.camera.position, Quaternion.identity);
                            pickup.extraTime = enemy.dropExtraTime;
                        }
                    }

                    return isDead;
                }
                break;
        }
        return false;
    }

    // true if dead
    public static bool InternalDamage(ref Character character, float damage)
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
                GameObject.Destroy(character.gameObject);
            }
            else if (character.id.type == IDType.Enemy)
            {
                Main.Singleton.levelStats.enemiesRemaining -= 1;
                // Allow for the pool to use it again.
                character.gameObject.SetActive(false);
            }
            return true;
        }
        return false;
    }
}
