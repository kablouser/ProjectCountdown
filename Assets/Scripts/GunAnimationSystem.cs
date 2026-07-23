using UnityEngine;

public static class GunAnimationSystem
{
    public static void Update(ref Character character)
    {
        if (0f < character.reloadCountdown && 0 < character.ActiveGun.reloadTime)
        {
            // reload animation
            // 0 to 1
            float animationTime = 1f - character.reloadCountdown / character.ActiveGun.reloadTime;
            float lerpTime = animationTime < .5f ? animationTime * 2f : 2f - animationTime * 2f;
            character.gunMagazine.localPosition = Vector3.Lerp(Vector3.zero, Vector3.down * 1.5f, lerpTime);
        }
        else
        {
            character.gunMagazine.localPosition = Vector3.zero;
        }

        if (0f < character.recoilCountdown)
        {
            character.recoilCountdown -= Time.deltaTime;
            if (character.recoilCountdown < 0f)
            {
                character.recoilCountdown = 0f;
            }

            // 0 to 1
            if (0f < character.recoilCountdownStart)
            {
                float animationTime = 1f - character.recoilCountdown / character.recoilCountdownStart;
                float lerpTime = animationTime < .5f ? animationTime * 2f : 2f - animationTime * 2f;
                character.gunBody.localPosition = Vector3.Lerp(Vector3.zero, Vector3.back * 2f, lerpTime);
            }
        }
        else
        {
            character.gunBody.localPosition = Vector3.zero;
        }
        if (character.shotThisFrame)
        {
            character.shotThisFrame = false;
            // recoil animation
            character.recoilCountdown = character.recoilCountdownStart = 0.3f *
                // time between shots
                60f / character.ActiveGun.roundsPerMin;
        }
    }
}
