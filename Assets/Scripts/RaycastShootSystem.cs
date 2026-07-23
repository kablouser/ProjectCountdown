using UnityEngine;

public static class RaycastShootSystem
{
    public static void Update(ref Character character, in LayerMask layerMask)
    {
        if (character.shootInput)
        {
            if (Physics.Raycast(
                character.camera.position,
                character.camera.forward,
                out RaycastHit raycastHit,
                1000f,
                layerMask))
            {
                Debug.Log("Bang " + raycastHit.collider.name);
            }
        }
        character.shootInput = false;
        character.reloadInput = false;
    }
}
