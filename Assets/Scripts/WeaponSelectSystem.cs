using UnityEngine;

public static class WeaponSelectSystem
{
    public static void Update(ref Character character)
    {
        if (character.weaponSelectInput < 0 || character.weaponSelectInput == character.currentSelectedWeapon ||
            // cannot switch weapons whilst shooting
            0 < character.shotCooldown)
        {
            return;
        }

        // Need to switch weapons
        character.currentSelectedWeapon = character.weaponSelectInput;
        // cancel reload
        character.reloadCountdown = 0f;
    }
}