using UnityEngine;

public enum IDType
{
    Invalid,
    Player,
    Enemy,
}

[System.Serializable]
public struct ID
{
    public IDType type;
    public int index;
    public int version;
}

[System.Serializable]
public struct Character
{
    public ID id;
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
    // true => lookPositionInput, false => lookInput
    [HideInInspector] public bool isUsingLookPositionInput;
    [HideInInspector] public Vector2 lookInput;
    [HideInInspector] public Vector3 lookPositionInput;
    [HideInInspector] public bool shootInput;
    [HideInInspector] public bool reloadInput;
    [HideInInspector] public bool jumpInput;
    /** -1 for no input this frame. */
    [HideInInspector] public int weaponSelectInput;
    #endregion

    [HideInInspector] public Vector2 currentLook;
    [HideInInspector] public float reloadCountdown;
    [HideInInspector] public float shotCooldown;
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

    public void Awake()
    {
        transform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        isStandingTracker = gameObject.GetComponent<IsStandingTracker>();
        currentLook = transform.rotation.eulerAngles.y * Vector2.up;
        currentHealth = maxHealth;

        // Start all guns with full ammo capacity
        for (int i = 0; i < gunStats.Length; i++)
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

    public void Awake()
    {
        character.id = new ID()
        {
            type = IDType.Player,
        };
        character.Awake();
        PlayerUI.Instance.GetTimer.SetRemainingTime(character);
    }
}

[System.Serializable]
public struct EnemyCharacter
{
    public Character character;
    public TimerUI healthBar;

    public static EnemyCharacter Default => new()
    {
        character = Character.Default,
    };

    public void Awake()
    {
        character.Awake();
        healthBar.SetRemainingTime(
            character.currentHealth, character.maxHealth);

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
    public float damage;
    [HideInInspector] public int currentAmmo;

    public static GunStat Default => new()
    {
        ammoCapacity = 5,
        reloadTime = 1.5f,
        roundsPerMin = 120,
        penetrationCount = 0,
        damage = 1f,
    };

    public void Reload()
    {
        currentAmmo = ammoCapacity;
    }
}
