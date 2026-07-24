using UnityEngine;

public static class WeaponSelectSystem
{
    public static void Update(ref Character character)
    {
        if (character.weaponSelectInput < 0 
            || character.weaponSelectInput == character.currentSelectedWeapon 
            // cannot switch weapons whilst shooting
            || 0 < character.shotCooldown
            // cannot switch if already switching.
            || character.timeToSwapCountdown > 0f)
        {
            return;
        }

        character.currentSelectedWeapon = character.weaponSelectInput;
        character.timeToSwapCountdown = Character.TimeToSwap;

        character.reloadCountdown = 0f;
    }
        
}