using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Main : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public static Main Singleton { get; private set; }

    public LayerMask shootLayerMask;
    public PlayerCharacter playerCharacter = PlayerCharacter.Default;
    public List<EnemyCharacter> enemyCharacters;

    InputSystem_Actions inputSystem_Actions;
    RaycastHit[] raycastHitCache;

    void Awake()
    {
        Singleton = this;
        if (inputSystem_Actions == null)
        {
            inputSystem_Actions = new InputSystem_Actions();
            inputSystem_Actions.Player.SetCallbacks(this);
        }
        if (raycastHitCache == null)
        {
            raycastHitCache = new RaycastHit[32];
        }
    }

    void Start()
    {
    }

    void OnEnable()
    {
        inputSystem_Actions.Player.Enable();
    }

    private void OnDisable()
    {
        inputSystem_Actions.Player.Disable();
    }

    void Update()
    {
        RaycastShootSystem.Update(ref playerCharacter.character, shootLayerMask, raycastHitCache);
        GunAnimationSystem.Update(ref playerCharacter.character);

        System.Span<EnemyCharacter> enemySpan = enemyCharacters.AsSpan();
        for (int i = 0; i < enemySpan.Length; i++)
        {
            GunAnimationSystem.Update(ref enemySpan[i].character);
        }
    }

    void FixedUpdate()
    {
        WalkingSystem.FixedUpdate(ref playerCharacter.character);

        System.Span<EnemyCharacter> enemySpan = enemyCharacters.AsSpan();
        for (int i = 0; i < enemySpan.Length; i++)
        {
            WalkingSystem.FixedUpdate(ref enemySpan[i].character);
        }
    }

    void InputSystem_Actions.IPlayerActions.OnMove(InputAction.CallbackContext context)
    {
        playerCharacter.character.moveInput = context.ReadValue<Vector2>();
    }

    void InputSystem_Actions.IPlayerActions.OnLook(InputAction.CallbackContext context)
    {
        playerCharacter.character.lookInput = context.ReadValue<Vector2>();
    }

    void InputSystem_Actions.IPlayerActions.OnShoot(InputAction.CallbackContext context)
    {
        playerCharacter.character.shootInput = context.action.GetButtonDown();
    }

    void InputSystem_Actions.IPlayerActions.OnReload(InputAction.CallbackContext context)
    {
        playerCharacter.character.reloadInput = context.action.GetButtonDown();
    }

    void InputSystem_Actions.IPlayerActions.OnJump(InputAction.CallbackContext context)
    {
        playerCharacter.character.jumpInput = context.action.GetButtonDown();
    }

    public void RegisterPlayer(PlayerCharacter player)
    {
        playerCharacter = player;
        playerCharacter.Start();
    }

    public void RegisterEnemy(EnemyCharacter enemyCharacter)
    {
        enemyCharacter.Start();
        enemyCharacters.Add(enemyCharacter);
    }
}
