using System.Collections.Generic;
using UnityEngine;

public struct RaycastHitDistanceComparer : IComparer<RaycastHit>
{
    int IComparer<RaycastHit>.Compare(RaycastHit x, RaycastHit y)
    {
        return x.distance.CompareTo(y.distance);
    }
}

public static class ShootSystem
{
    public static float ShootRange = 1000f;
    public static float MeleeRange = 2.0f;

    public static void Update(ref Character character, in LayerMask layerMask, RaycastHit[] raycastHitCache)
    {
        if (character.timeToSwapCountdown > 0)
        {
            // if swapping gun don't allow reload.
            return;
        }
        
        if (0 < character.reloadCountdown)
        {
            character.reloadCountdown -= Time.deltaTime;
            if (character.reloadCountdown < 0)
            {
                character.reloadCountdown = 0;
                character.ActiveGun.Reload();
                character.shotCooldown = 0;

                if (!character.ActiveGun.isMelee)
                {
                    character.audioSource.PlayOneShot(Main.Singleton.weaponReloadEndSFX);
                }
            }
        }
        if (0 < character.shotCooldown)
        {
            character.shotCooldown -= Time.deltaTime;
            if (character.shotCooldown < 0)
            {
                character.shotCooldown = 0;
            }
        }

        if (0 < character.reloadCountdown)
        {
            // if waiting on reload, don't allow shooting or more reloading
        }
        // Reload if we press reload or we have no bullets in the mag.
        else if ((character.reloadInput || (character.ActiveGun.currentAmmo == 0 && character.shotCooldown == 0))
                 && character.ActiveGun.currentAmmo < character.ActiveGun.ammoCapacity)
        {
            character.reloadCountdown = character.ActiveGun.reloadTime;
            if (!character.ActiveGun.isMelee)
            {
                character.audioSource.PlayOneShot(Main.Singleton.weaponReloadStartSFX);
            }
        }
        else if (character.shootInput)
        {
            if (0 < character.ActiveGun.currentAmmo)
            {
                // is roundsPerMin exceeded?
                if (0 < character.shotCooldown)
                {
                    return;
                }

                float raycastDistance = character.ActiveGun.isMelee ? MeleeRange : ShootRange;
                character.ActiveGun.currentAmmo--;
                character.shotThisFrame = true;
                character.shotCooldown = 60f / character.ActiveGun.roundsPerMin;
                int hitCount;
                if (0 < character.ActiveGun.penetrationCount)
                {
                     hitCount = Physics.RaycastNonAlloc(
                        character.camera.position,
                        character.camera.forward,
                        raycastHitCache,
                        raycastDistance,
                        layerMask);
                    if (character.ActiveGun.penetrationCount + 1 < hitCount)
                    {
                        System.Array.Sort(raycastHitCache, 0, hitCount, new RaycastHitDistanceComparer());
                        hitCount = character.ActiveGun.penetrationCount + 1;
                    }
                }
                else
                {
                    if (Physics.Raycast(
                        character.camera.position,
                        character.camera.forward,
                        out RaycastHit raycastHit,
                        raycastDistance,
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
                    if (raycastHitCache[hitI].rigidbody != null)
                    {
                        IProxy iproxy = raycastHitCache[hitI].rigidbody.GetComponent<IProxy>();
                        if (iproxy != null)
                        {
                            DamageCharacter.Damage(iproxy.GetID(), character.ActiveGun.damage, TimeAdjustmentReason.DAMAGE);

                            if (character.id.type == IDType.Player)
                            {
                                character.audioSource.PlayOneShot(Main.Singleton.hitMarkerSFX);
                            }
                        }
                    }
                }

                character.audioSource.PlayRandomOneShot(Main.Singleton.shootSFXs);
            }
            else
            {
                // play out of ammo SFX here
            }
        }
    }
}
