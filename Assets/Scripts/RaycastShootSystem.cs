using UnityEngine;

public static class RaycastShootSystem
{
    public static void Update(ref Character character, in LayerMask layerMask, RaycastHit[] raycastHitCache)
    {
        if (0 < character.reloadCountdown)
        {
            character.reloadCountdown -= Time.deltaTime;
            if (character.reloadCountdown < 0)
            {
                character.reloadCountdown = 0;
                character.currentAmmo = character.gunStat.ammoCapacity;
                Debug.Log("Reloaded");
            }
        }

        if (0 < character.reloadCountdown)
        {
            // if waiting on reload, don't allow shooting or more reloading
        }
        else if (character.reloadInput && character.currentAmmo < character.gunStat.ammoCapacity)
        {
            character.reloadCountdown = character.gunStat.reloadTime;
            Debug.Log("Begin Reload");
        }
        else if (character.shootInput)
        {
            if (0 < character.currentAmmo)
            {
                const float MaxDistance = 1000f;
                character.currentAmmo--;
                int hitCount;
                if (0 < character.gunStat.penetrationCount)
                {
                     hitCount = Physics.RaycastNonAlloc(
                        character.camera.position,
                        character.camera.forward,
                        raycastHitCache,
                        MaxDistance,
                        layerMask);
                    if (character.gunStat.penetrationCount + 1 < hitCount)
                    {
                        hitCount = character.gunStat.penetrationCount + 1;
                    }
                }
                else
                {
                    if (Physics.Raycast(
                        character.camera.position,
                        character.camera.forward,
                        out RaycastHit raycastHit,
                        MaxDistance,
                        layerMask))
                    {
                        raycastHitCache[0] = raycastHit;
                        hitCount = 1;
                    }
                    else
                    {
                        hitCount = 0;
                    }
                }

                for (int hitI = 0; hitI < hitCount; hitI++)
                {
                    Debug.Log("Bang " + raycastHitCache[hitI].collider.name);
                }
            }
            else
            {
                // play out of ammo SFX here
            }
        }

        character.shootInput = false;
        character.reloadInput = false;
    }
}
