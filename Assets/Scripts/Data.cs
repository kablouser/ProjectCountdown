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
    [HideInInspector] public MeshRenderer gunMesh;
    [HideInInspector] public Transform transform;
    [HideInInspector] public Rigidbody rigidbody;
    [HideInInspector] public IsStandingTracker isStandingTracker;
    [Header("Settings")]
    public float maxHealth;
    public float moveSpeed;
    public float turnSpeed;
    public float jumpHeight;

    public ref GunStat ActiveGun => ref gunStats[currentSelectedWeapon];

    [HideInInspector] public GunStat[] gunStats;

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
    };

    public void Awake()
    {
        transform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        isStandingTracker = gameObject.GetComponent<IsStandingTracker>();
        currentLook = transform.rotation.eulerAngles.y * Vector2.up;
        currentHealth = maxHealth;
        gunMesh = gunBody.gameObject.GetComponent<MeshRenderer>();

        // Start all guns with full ammo capacity
        for (int i = 0; i < gunStats.Length; i++)
        {
            gunStats[i].Reload();
        }

        if (ActiveGun.weaponMaterial)
        {
            gunMesh.sharedMaterial = ActiveGun.weaponMaterial;
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

    public static EnemyCharacter Default => new()
    {
        character = Character.Default,
        dropExtraTime = 1f,
        dropExtraTimeChance = 0.5f,
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
    public int ammoCapacity;
    public float reloadTime;
    public float roundsPerMin;
    // number of colliders this rounds penetrate through
    public int penetrationCount;
    public Sprite weaponImage;
    public float damage;
    // melee is effectively very short ranged gun
    public bool isMelee;
    [HideInInspector] public int currentAmmo;
    public Material weaponMaterial;

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
