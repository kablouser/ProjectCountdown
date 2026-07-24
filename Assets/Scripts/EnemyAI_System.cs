using UnityEngine;

public static class EnemyAI_System
{
    public static void Update(ref EnemyCharacter enemy)
    {
        Main main = Main.Singleton;
        ref Character character = ref enemy.character;

        if (main.isPlayerAlive)
        {
            character.isUsingLookPositionInput = true;
            character.lookPositionInput = main.playerCharacter.character.transform.position;
            character.moveInput = new Vector2(0, 1);
            character.shootInput =
                Vector3.Distance(character.transform.position, character.lookPositionInput) <=
                (character.ActiveGun.isMelee ? ShootSystem.MeleeRange : ShootSystem.ShootRange);
            character.jumpInput = false;
            character.reloadInput = character.ActiveGun.currentAmmo == 0;
        }
        else
        {
            character.isUsingLookPositionInput = false;
            character.lookInput = new Vector2();
            character.moveInput = new Vector2();
            character.shootInput = false;
            character.jumpInput = false;
            character.reloadInput = false;
        }
    }
}
