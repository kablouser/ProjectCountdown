using UnityEngine;

public static class WalkingSystem
{
    public static void FixedUpdate(ref Character character)
    {
        character.currentLook += new Vector2(-character.lookInput.y, character.lookInput.x) * character.turnSpeed;
        character.currentLook.x = Mathf.Clamp(character.currentLook.x, -90, 90);
        Quaternion yRotation = Quaternion.Euler(0, character.currentLook.y, 0);
        Quaternion xRotation = Quaternion.Euler(character.currentLook.x, 0, 0);

        Vector3 velocity = character.rigidbody.linearVelocity;
        Vector3 horizontalVelocity = velocity;
        horizontalVelocity.y = 0;

        Vector3 velocityChange =
            yRotation *
            new Vector3(character.moveInput.x, 0, character.moveInput.y) *
            character.moveSpeed -
            horizontalVelocity;
        character.rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        character.rigidbody.rotation = yRotation;

        character.camera.transform.localRotation = xRotation;

        if (character.jumpInput &&
            character.isStandingTracker.previousIsStanding)
        {
            float jumpVelocity = Mathf.Sqrt(2 * 9.81f * character.jumpHeight);
            character.rigidbody.AddForce(0, jumpVelocity - velocity.y, 0, ForceMode.VelocityChange);
        }
        character.jumpInput = false;
    }
}
