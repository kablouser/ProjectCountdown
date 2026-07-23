using UnityEngine;

[System.Serializable]
public struct Player
{
    public GameObject gameObject;
    public Transform camera;
    public Transform transform;
    public Rigidbody rigidbody;

    public float health;
    public float moveSpeed;
    public float turnSpeed;

    public Vector2 moveInput;
    public Vector2 lookInput;
    public Vector2 currentLook;

    public void Start()
    {
        transform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        currentLook = transform.rotation.eulerAngles.y * Vector2.up;
    }
}

public static class WalkingSystem
{
    public static void FixedUpdate(ref Player player)
    {
        player.currentLook += new Vector2(-player.lookInput.y, player.lookInput.x) * player.turnSpeed;
        player.currentLook.x = Mathf.Clamp(player.currentLook.x, -90, 90);
        Quaternion yRotation = Quaternion.Euler(0, player.currentLook.y, 0);
        Quaternion xRotation = Quaternion.Euler(player.currentLook.x, 0, 0);

        Vector3 horizontalVelocity = player.rigidbody.linearVelocity;
        horizontalVelocity.y = 0;

        Vector3 velocityChange =
            yRotation *
            new Vector3(player.moveInput.x, 0, player.moveInput.y) *
            player.moveSpeed -
            horizontalVelocity;
        player.rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        player.rigidbody.rotation = yRotation;

        player.camera.transform.localRotation = xRotation;
    }
}
