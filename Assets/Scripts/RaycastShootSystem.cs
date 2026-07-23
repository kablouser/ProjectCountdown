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
                character.ActiveGun.Reload();
                character.lastShotTime = -1000f;
                Debug.Log("Reloaded");
            }
        }

        if (0 < character.reloadCountdown)
        {
            // if waiting on reload, don't allow shooting or more reloading
        }
        else if (character.reloadInput && character.ActiveGun.currentAmmo < character.ActiveGun.ammoCapacity)
        {
            character.reloadCountdown = character.ActiveGun.reloadTime;
            Debug.Log("Begin Reload");
        }
        else if (character.shootInput)
        {
            if (0 < character.ActiveGun.currentAmmo)
            {
                // is RPM exceeded?
                float minTimeBetweenShots = 60f / character.ActiveGun.roundsPerMin;
                float timeBetweenLastShot = Time.time - character.lastShotTime;
                if (timeBetweenLastShot < minTimeBetweenShots)
                {
                    goto updateEnd;
                }

                const float MaxDistance = 1000f;
                character.ActiveGun.currentAmmo--;
                character.shotThisFrame = true;
                character.lastShotTime = Time.time;
                int hitCount;
                if (0 < character.ActiveGun.penetrationCount)
                {
                     hitCount = Physics.RaycastNonAlloc(
                        character.camera.position,
                        character.camera.forward,
                        raycastHitCache,
                        MaxDistance,
                        layerMask);
                    if (character.ActiveGun.penetrationCount + 1 < hitCount)
                    {
                        hitCount = character.ActiveGun.penetrationCount + 1;
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

    updateEnd:
        character.shootInput = false;
        character.reloadInput = false;
        PlayerUI.Instance.GetWeaponUI.SetAmmo(character.gunStat,
            character.currentAmmo, character.gunStat.ammoCapacity);
    }
}
