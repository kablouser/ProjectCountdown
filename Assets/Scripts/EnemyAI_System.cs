using UnityEngine;

public static class EnemyAI_System
{
    public static void Update(ref EnemyCharacter enemy)
    {
        Main main = Main.Singleton;
        if (!main.isPlayerValid)
        {
            return;
        }

        ref Character character = ref enemy.character;
        character.isUsingLookPositionInput = true;
        character.lookPositionInput = main.playerCharacter.character.transform.position;
        character.moveInput = new Vector2(0, 1);
        character.shootInput = true;
        character.jumpInput = false;
        character.reloadInput = character.ActiveGun.currentAmmo == 0;
    }
}
