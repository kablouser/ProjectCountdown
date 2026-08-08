using System;
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

    public static ID Player => new ID { type = IDType.Player };

    public bool IsEqual(ID other)
    {
        return type == other.type &&
            index == other.index &&
            version == other.version;
    }
}

public enum MovementType { Walking, Flying }

[System.Serializable]
public struct Character
{
    public ID id;
    [Header("Character - Must assign")]
    public GameObject gameObject;
    public Transform camera;
    public Transform gunMagazine;
    public Transform gunBody;
    public AudioSource audioSource;
    [HideInInspector] public MeshRenderer gunMesh;
    [HideInInspector] public MeshFilter gunBodyMesh;
    [HideInInspector] public MeshFilter gunAmmoMesh;
    [HideInInspector] public Transform transform;
    [HideInInspector] public Rigidbody rigidbody;
    [HideInInspector] public IsStandingTracker isStandingTracker;
    [Header("Settings")]
    public float maxHealth;
    public float moveSpeed;
    public float turnSpeed;
    public bool invertLookY;
    public float jumpHeight;
    public MovementType movementType;
    public float flyHeight;

    public bool IsActiveGunValid()
    {
        return currentSelectedWeapon < gunStats.Length;
    }
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
    public const float TimeToSwap = 0.2f;
    [HideInInspector] public float timeToSwapCountdown;
        
    

    public static Character Default => new()
    {
        maxHealth = 1,
        moveSpeed = 3,
        turnSpeed = 1,
        jumpHeight = 1,
        currentSelectedWeapon = 0,
        movementType = MovementType.Walking,
        flyHeight = 3f,
    };

    public void Awake()
    {
        transform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        isStandingTracker = gameObject.GetComponent<IsStandingTracker>();
        currentLook = transform.rotation.eulerAngles.y * Vector2.up;
        currentHealth = maxHealth;
        gunMesh = gunBody.gameObject.GetComponent<MeshRenderer>();
        gunBodyMesh = gunBody.gameObject.GetComponent<MeshFilter>();
        gunAmmoMesh = gunMagazine.gameObject.GetComponent<MeshFilter>();
        weaponSelectInput = -1;

        // Start all guns with full ammo capacity
        for (int i = 0; i < gunStats.Length; i++)
        {
            gunStats[i].Reload();
        }

        GunAnimationSystem.SetGunVisuals(this, ActiveGun);
    }
}

[System.Serializable]
public struct PlayerCharacter
{
    public Character character;
    public bool pauseInput;

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
        character.gunStats = PlayerUI.Instance.shopScreen.GetGunStats();
        character.Awake();
        PlayerUI.Instance.healthBar.SetRemainingTime(character);
    }
}

[System.Serializable]
public struct EnemyCharacter
{
    public Character character;
    public BarUI healthBar;
    public float dropExtraTime;
    public float dropExtraTimeChance;
    public bool isAvoidingObstacle;
    public bool chosenObstacleAvoidanceDirection;
    public float rareDropExtraTime;
    public float rareDropExtraTimeChance;

    public static EnemyCharacter Default => new()
    {
        character = Character.Default,
        dropExtraTime = 1f,
        dropExtraTimeChance = 0.5f,
        rareDropExtraTime = 5.0f,
        rareDropExtraTimeChance = 0.2f
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
    public string name;
    public bool isUnlocked;
    public bool automatic;
    public int ammoCapacity;
    public float reloadTime;
    public float roundsPerMin;
    // number of colliders this rounds penetrate through
    public int penetrationCount;
    public Sprite weaponImage;
    public float damage;
    // melee is effectively very short ranged gun
    public float range;
    [HideInInspector] public int currentAmmo;
    public Material weaponMaterial;
    public Mesh weaponBodyMesh;
    public Mesh weaponAmmoMesh;
    public float uniformWeaponScale;
    public float kickbackAmount;
    // if null, we will use raycast
    public Projectile projectilePrefab;
    public bool isProjectileHoming;

    public static GunStat Default => new()
    {
        ammoCapacity = 5,
        reloadTime = 1.5f,
        roundsPerMin = 120,
        penetrationCount = 0,
        damage = 1f,
        range = 1000f,
    };

    public void Reload()
    {
        currentAmmo = ammoCapacity;
    }
}

[Serializable]
public struct LevelStats
{
    public int GetEnemyCount(int level)
    {
        double doubleEnemyCount = baseEnemyCount * Math.Pow(enemyIncreaseMultiplier, level);
        int intEnemyCount = (int)doubleEnemyCount;
        if (1f < enemyIncreaseMultiplier &&
            intEnemyCount < baseEnemyCount)
        {
            // underflowed
            intEnemyCount = int.MaxValue;
        }
        return intEnemyCount;
    }

    public float GetEnemySpeed(int level)
    {
        float maxSpeed = Main.Singleton.playerCharacter.character.moveSpeed - 2;

        double enemySpeed = baseEnemySpeed * Math.Pow(enemySpeedIncreaseMultiplier, level);
        float actualSpeed = (float)Math.Min(maxSpeed, enemySpeed);
        
        return actualSpeed;
    }

    public int GetRandomEnemyType(int level)
    {
        if (level < 1)
        {
            return 0;
        }
        else
        {
            return UnityEngine.Random.value < enemyType1SpawnRate ? 1 : 0;
        }
    }
    
    [SerializeField] private int baseEnemyCount;
    [SerializeField] private float enemyIncreaseMultiplier;
    [SerializeField] private float baseEnemySpeed;
    [SerializeField] private float enemySpeedIncreaseMultiplier;
    [HideInInspector] public int enemiesRemaining;
    
    public float playerStartHealth;
    public float timeBetweenEnemySpawning;

    [Range(0,1)] public float enemyType1SpawnRate;
}