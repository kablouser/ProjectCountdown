using UnityEngine;

[System.Serializable]
public struct Character
{
    [Header("Must assign")]
    public GameObject gameObject;
    public Transform camera;
    [HideInInspector] public Transform transform;
    [HideInInspector] public Rigidbody rigidbody;
    [HideInInspector] public IsStandingTracker isStandingTracker;
    [Header("Settings")]
    public float health;
    public float moveSpeed;
    public float turnSpeed;
    public float jumpHeight;
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 lookInput;
    [HideInInspector] public bool shootInput;
    [HideInInspector] public bool reloadInput;
    [HideInInspector] public bool jumpInput;
    [HideInInspector] public Vector2 currentLook;

    public static Character Default => new()
    {
        health = 1,
        moveSpeed = 3,
        turnSpeed = 1,
        jumpHeight = 1,
    };

    public void Start()
    {
        transform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        isStandingTracker = gameObject.GetComponent<IsStandingTracker>();
        currentLook = transform.rotation.eulerAngles.y * Vector2.up;
    }
}
