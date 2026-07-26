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

                if (character.id.type == IDType.Player)
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
            if (character.id.type == IDType.Player)
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

                float raycastDistance = character.ActiveGun.range;
                character.ActiveGun.currentAmmo--;
                character.shotThisFrame = true;
                character.shotCooldown = 60f / character.ActiveGun.roundsPerMin;

                if (character.ActiveGun.projectilePrefab == null)
                {
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
                                float damageToApply = character.ActiveGun.damage;
                                bool headshot = raycastHitCache[hitI].collider.name == "HeadCollider";
                                if (headshot)
                                {
                                    damageToApply *= 1.5f;
                                }
                                DamageCharacter.Damage(iproxy.GetID(), damageToApply, TimeAdjustmentReason.DAMAGE, character.id.type, headshot);

                                if (character.id.type == IDType.Player)
                                {
                                    character.audioSource.PlayOneShot(Main.Singleton.hitMarkerSFX);
                                }
                            }
                        }
                    }
                    // draw line for player
                    if (character.id.type == IDType.Player)
                    {
                        LineRenderer lineRender = Main.Singleton.lineRenderer;
                        lineRender.positionCount += 2;
                        int count = lineRender.positionCount;
                        Vector3 start = Main.Singleton.playerCharacter.character.gunBody.position;
                        lineRender.SetPosition(count - 2, start);

                        if (hitCount == 0)
                        {
                            lineRender.SetPosition(count - 1, start + Main.Singleton.playerCharacter.character.camera.forward * Main.Singleton.playerCharacter.character.ActiveGun.range);
                        }
                        else
                        {
                            lineRender.SetPosition(count - 1, raycastHitCache[hitCount - 1].point);
                        }
                    }
                }
                else
                {
                    Projectile projectile = GameObject.Instantiate(character.ActiveGun.projectilePrefab,
                        character.camera.transform.position,
                        character.camera.rotation);
                    projectile.damage = character.ActiveGun.damage;
                    projectile.source = character.id;
                    // step out of the collider of the firing character
                    projectile.transform.Translate(character.transform.forward * (0.51f + projectile.radiusApproximate), Space.World);

                    if (character.ActiveGun.isProjectileHoming &&
                        character.id.type != IDType.Player &&
                        Main.Singleton.isPlayerAlive &&
                        Main.Singleton.playerCharacter.character.camera != null)
                    {
                        projectile.homingTarget = Main.Singleton.playerCharacter.character.camera;
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
