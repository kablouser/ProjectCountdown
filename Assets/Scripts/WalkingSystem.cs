using JetBrains.Annotations;
using UnityEngine;

public static class WalkingSystem
{
    public static void Update(ref Character character)
    {
        if (character.isUsingLookPositionInput)
        {
            Quaternion desiredQuat = Quaternion.LookRotation(character.lookPositionInput - character.transform.position, Vector3.up);
            Quaternion currentQuat = Quaternion.Euler(character.currentLook.x, character.currentLook.y, 0);
            character.currentLook = Quaternion.Slerp(currentQuat, desiredQuat, character.turnSpeed * Time.deltaTime).eulerAngles;
            character.isUsingLookPositionInput = false;
        }
        else
        {
            float lookY = character.invertLookY ? character.lookInput.y : -character.lookInput.y;
            character.currentLook += new Vector2(lookY, character.lookInput.x) * character.turnSpeed * Time.deltaTime;
            character.currentLook.x = Mathf.Clamp(character.currentLook.x, -90, 90);
        }
        Quaternion yRotation = Quaternion.Euler(0, character.currentLook.y, 0);
        Quaternion xRotation = Quaternion.Euler(character.currentLook.x, 0, 0);
        character.rigidbody.rotation = yRotation;
        character.camera.transform.localRotation = xRotation;
    }

    public static void FixedUpdate(ref Character character)
    {
        Quaternion yRotation = Quaternion.Euler(0, character.currentLook.y, 0);

        Vector3 velocity = character.rigidbody.linearVelocity;
        Vector3 horizontalVelocity = velocity;
        horizontalVelocity.y = 0;

        Vector3 velocityChange =
            yRotation *
            new Vector3(character.moveInput.x, 0, character.moveInput.y) *
            character.moveSpeed -
            horizontalVelocity;
        if (1f < velocityChange.sqrMagnitude)
        {
            velocityChange = velocityChange.normalized * 1f;
        }
        character.rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        switch (character.movementType)
        {
            case MovementType.Walking:
                if (character.jumpInput &&
                    character.isStandingTracker.previousIsStanding)
                {
                    float jumpVelocity = Mathf.Sqrt(2 * 9.81f * character.jumpHeight);
                    character.rigidbody.AddForce(0, jumpVelocity - velocity.y, 0, ForceMode.VelocityChange);
                }
                break;
            case MovementType.Flying:
                float flyY = character.flyHeight - character.transform.position.y;
                float velocityChangeY = flyY - velocity.y;

                if (Mathf.Abs(velocity.y) < 2f)
                {
                    // create a bobbing motion so its not perfectly still
                    velocityChangeY = flyY;
                }

                character.rigidbody.AddForce(velocityChangeY * Vector2.up, ForceMode.VelocityChange);
                break;
        }
    }
}
