using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

struct DrawArrowLifetime
{
    public Vector3 from;
    public Vector3 to;
    public float lifeEnd;
}


public class Main : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public static Main Singleton { get; private set; }

    public LayerMask shootLayerMask;
    public bool isPlayerValid;
    public PlayerCharacter playerCharacter = PlayerCharacter.Default;
    public VersionedList<EnemyCharacter> enemyCharacters;

    InputSystem_Actions inputSystem_Actions;
    RaycastHit[] raycastHitCache;

    List<DrawArrowLifetime> drawArrows;

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
        drawArrows = new List<DrawArrowLifetime>();
        enemyCharacters.type = IDType.Enemy;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
        if (isPlayerValid)
        {
            WeaponSelectSystem.Update(ref playerCharacter.character);
            PlayerUI.Instance.GetWeaponUI.SetAmmo(playerCharacter.character.ActiveGun);
            RaycastShootSystem.Update(ref playerCharacter.character, shootLayerMask, raycastHitCache);
            GunAnimationSystem.Update(ref playerCharacter.character);
            WalkingSystem.Update(ref playerCharacter.character);
        }

        foreach (ref EnemyCharacter enemy in enemyCharacters)
        {
            EnemyAI_System.Update(ref enemy);
            RaycastShootSystem.Update(ref enemy.character, shootLayerMask, raycastHitCache);
            GunAnimationSystem.Update(ref enemy.character);
            WalkingSystem.Update(ref enemy.character);
        }
        
        ResetOneTimeInputs();
    }

    void FixedUpdate()
    {
        if (isPlayerValid)
        {
            WalkingSystem.FixedUpdate(ref playerCharacter.character);
        }

        foreach (ref EnemyCharacter enemy in enemyCharacters)
        {
            WalkingSystem.FixedUpdate(ref enemy.character);
        }
    }
    private void OnDrawGizmos()
    {
        if (drawArrows == null) return;
        for (int i = 0; i < drawArrows.Count; i++)
        {
            if (Time.time < drawArrows[i].lifeEnd)
            {
                drawArrows.RemoveAt(i);
                i--;
                continue;
            }
            GizmosMore.DrawArrow(drawArrows[i].from, drawArrows[i].to);
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

    public void AwakePlayer(PlayerCharacter player)
    {
        isPlayerValid = true;
        playerCharacter = player;
        playerCharacter.Awake();
    }

    public ID AwakeEnemy(EnemyCharacter enemyCharacter)
    {
        enemyCharacter.Awake();
        return enemyCharacters.Add(enemyCharacter);
    }

    public void DestroyPlayer()
    {
        isPlayerValid = false;
    }

    public bool DestroyEnemy(ID id)
    {
        return enemyCharacters.Remove(id);
    }

    public void DrawArrowGizmo(Vector3 from, Vector3 to, float duration)
    {
        drawArrows.Add(new DrawArrowLifetime { from = from, to = to, lifeEnd = Time.time + duration });
    }

    void InputSystem_Actions.IPlayerActions.OnFirstWeapon(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            playerCharacter.character.weaponSelectInput = 0;
        }
    }

    void InputSystem_Actions.IPlayerActions.OnSecondWeapon(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            playerCharacter.character.weaponSelectInput = 1;
        }
    }

    private void ResetOneTimeInputs()
    {
        playerCharacter.character.weaponSelectInput = -1;
    }
}
