using UnityEngine;

public static class WeaponSelectSystem
{
    public static void Update(ref Character character)
    {
        if (character.weaponSelectInput < 0 || character.weaponSelectInput == character.currentSelectedWeapon)
        {
            return;
        }
        
        // Need to switch weapons
        character.currentSelectedWeapon = character.weaponSelectInput;
        Debug.Log("Switched to weapon " + character.currentSelectedWeapon);
    }
        
}