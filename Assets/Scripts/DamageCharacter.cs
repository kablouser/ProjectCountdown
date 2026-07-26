using UnityEngine;

public static class DamageCharacter
{
    // true if dead
    public static bool Damage(in ID id, float damage, TimeAdjustmentReason reason, bool headshot = false)
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
                    if (isDead)
                    {
                        PlayerUI.Instance.gameOverScreen.SetGameOverReason(reason);
                    }

                    if (reason == TimeAdjustmentReason.DAMAGE && 0f < damage)
                    {
                        main.playerCharacter.character.audioSource.PlayRandomOneShot(main.playerDamageSFXs);
                    }
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

                    PlayerUI.Instance.ShowHitMarker(headshot);


                    if (isDead)
                    {
                        float changeIncrease = 0.0f;
                        bool increaseChance = main.playerTimeLeft < 30.0f;
                        if (increaseChance)
                        {
                            changeIncrease = (1 - (main.playerTimeLeft / 30.0f)) * (1 - enemy.dropExtraTimeChance);
                        }

                        if (Random.value <= (enemy.dropExtraTimeChance + changeIncrease))
                        {
                            PickUp pickup = GameObject.Instantiate(main.pickUpExtraTimePrefab, enemy.character.camera.position, Quaternion.identity);
                            pickup.extraTime = enemy.dropExtraTime;
                        }
                        // cannot use character audio source because it will be destroyed
                        main.PlayOneShotSFX(enemy.character.camera.position, main.enemyKilledSFXs.GetRandom());
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
