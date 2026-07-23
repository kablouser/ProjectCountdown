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
    public GunStat gunStat;
    //input begin
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 lookInput;
    [HideInInspector] public bool shootInput;
    [HideInInspector] public bool reloadInput;
    [HideInInspector] public bool jumpInput;
    //input end
    [HideInInspector] public Vector2 currentLook;
    [HideInInspector] public int currentAmmo;
    [HideInInspector] public float reloadCountdown;

    public static Character Default => new()
    {
        health = 1,
        moveSpeed = 3,
        turnSpeed = 1,
        jumpHeight = 1,
        gunStat = GunStat.Default,
    };

    public void Start()
    {
        transform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        isStandingTracker = gameObject.GetComponent<IsStandingTracker>();
        currentLook = transform.rotation.eulerAngles.y * Vector2.up;
        currentAmmo = gunStat.ammoCapacity;
    }
}

[System.Serializable]
public struct GunStat
{
    public int ammoCapacity;
    public float reloadTime;
    public float roundsPerMin;
    // number of colliders this rounds penetrate through
    public int penetrationCount;

    public static GunStat Default => new()
    {
        ammoCapacity = 5,
        reloadTime = 2,
        roundsPerMin = 60,
        penetrationCount = 0,
    };
}
