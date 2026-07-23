using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public Character player = Character.Default;
    public LayerMask shootLayerMask;

    InputSystem_Actions inputSystem_Actions;
    RaycastHit[] raycastHitCache;

    void Awake()
    {
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
        player.Start();
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
        RaycastShootSystem.Update(ref player, shootLayerMask, raycastHitCache);
    }

    void FixedUpdate()
    {
        WalkingSystem.FixedUpdate(ref player);
    }

    void InputSystem_Actions.IPlayerActions.OnMove(InputAction.CallbackContext context)
    {
        player.moveInput = context.ReadValue<Vector2>();
    }

    void InputSystem_Actions.IPlayerActions.OnLook(InputAction.CallbackContext context)
    {
        player.lookInput = context.ReadValue<Vector2>();
    }

    void InputSystem_Actions.IPlayerActions.OnShoot(InputAction.CallbackContext context)
    {
        player.shootInput = context.action.GetButtonDown();
    }

    void InputSystem_Actions.IPlayerActions.OnReload(InputAction.CallbackContext context)
    {
        player.reloadInput = context.action.GetButtonDown();
    }

    void InputSystem_Actions.IPlayerActions.OnJump(InputAction.CallbackContext context)
    {
        player.jumpInput = context.action.GetButtonDown();
    }
}
