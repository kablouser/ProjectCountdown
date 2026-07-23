using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public Player player;

    InputSystem_Actions inputSystem_Actions;

    void Awake()
    {
        if (inputSystem_Actions == null)
        {
            inputSystem_Actions = new InputSystem_Actions();
            inputSystem_Actions.Player.SetCallbacks(this);
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

    void InputSystem_Actions.IPlayerActions.OnAttack(InputAction.CallbackContext context)
    {
    }

    void InputSystem_Actions.IPlayerActions.OnInteract(InputAction.CallbackContext context)
    {
    }

    void InputSystem_Actions.IPlayerActions.OnCrouch(InputAction.CallbackContext context)
    {
    }

    void InputSystem_Actions.IPlayerActions.OnJump(InputAction.CallbackContext context)
    {
    }

    void InputSystem_Actions.IPlayerActions.OnPrevious(InputAction.CallbackContext context)
    {
    }

    void InputSystem_Actions.IPlayerActions.OnNext(InputAction.CallbackContext context)
    {
    }

    void InputSystem_Actions.IPlayerActions.OnSprint(InputAction.CallbackContext context)
    {
    }
}
