using UnityEngine;

[System.Serializable]
public struct Character
{
    [Header("Character - Must assign")]
    public GameObject gameObject;
    public Transform camera;
    public Transform gunMagazine;
    public Transform gunBody;
    [HideInInspector] public Transform transform;
    [HideInInspector] public Rigidbody rigidbody;
    [HideInInspector] public IsStandingTracker isStandingTracker;
    [Header("Settings")]
    public float maxHealth;
    public float moveSpeed;
    public float turnSpeed;
    public float jumpHeight;

    public ref GunStat ActiveGun => ref gunStats[currentSelectedWeapon];

    public GunStat[] gunStats;
    
    #region Input 
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 lookInput;
    [HideInInspector] public bool shootInput;
    [HideInInspector] public bool reloadInput;
    [HideInInspector] public bool jumpInput;
    /** -1 for no input this frame. */
    [HideInInspector] public int weaponSelectInput;
    #endregion
    
    [HideInInspector] public Vector2 currentLook;
    [HideInInspector] public float reloadCountdown;
    [HideInInspector] public float lastShotTime;
    [HideInInspector] public bool shotThisFrame;
    [HideInInspector] public float recoilCountdown;
    [HideInInspector] public float recoilCountdownStart;
    [HideInInspector] public float currentHealth;

    [HideInInspector] public int currentSelectedWeapon;

    public static Character Default => new()
    {
        maxHealth = 1,
        moveSpeed = 3,
        turnSpeed = 1,
        jumpHeight = 1,
        currentSelectedWeapon = 0,
    };

    public void Start()
    {
        transform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        isStandingTracker = gameObject.GetComponent<IsStandingTracker>();
        currentLook = transform.rotation.eulerAngles.y * Vector2.up;
        lastShotTime = -1000f;

        // Start all guns with full ammo capacity
        for (int i = 0; i < gunStats.Length; i ++)
        {
            gunStats[i].Reload();
        }
    }
}

[System.Serializable]
public struct PlayerCharacter
{
    public Character character;

    public static PlayerCharacter Default => new()
    {
        character = Character.Default,
    };

    public void Start()
    {
        character.Start();
    }
}

[System.Serializable]
public struct EnemyCharacter
{
    public Character character;

    public static EnemyCharacter Default => new()
    {
        character = Character.Default,
    };

    public void Start()
    {
        character.Start();
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
    public Sprite weaponImage;
    
    [HideInInspector] public int currentAmmo;

    public static GunStat Default => new()
    {
        ammoCapacity = 5,
        reloadTime = 1.5f,
        roundsPerMin = 120,
        penetrationCount = 0,
    };

    public void Reload()
    {
        currentAmmo = ammoCapacity;
    } 
}
