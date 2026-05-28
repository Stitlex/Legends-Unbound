using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    public event EventHandler OnPlayerAttack;
    public event EventHandler OnSlot1Pressed;
    public event EventHandler OnSlot2Pressed;
    public event EventHandler OnInventoryToggled;
    public event EventHandler OnPausePressed;
    public event EventHandler OnCharacterMenuToggled;
    public event EventHandler OnPlayerDash;

    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Enable();
        playerInputActions.QuickSlots.Enable();
        playerInputActions.Combat.Enable();
        playerInputActions.Player.Dash.performed += PlayerDashPerformed;

        playerInputActions.Combat.Attack.started += PlayerAttack_started;
        playerInputActions.QuickSlots.Slot1.performed += OnSlot1Performed;
        playerInputActions.QuickSlots.Slot2.performed += OnSlot2Performed;
        playerInputActions.Player.ToggleInventory.performed += OnToggleInventoryPerformed;
        playerInputActions.Player.Pause.performed += Pause_performed;
        playerInputActions.Player.ToggleCharacterMenu.performed += OnCharacterMenuPerformed;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Disable();
        playerInputActions.QuickSlots.Disable();
        playerInputActions.Combat.Disable();
        playerInputActions.Dispose();
        playerInputActions.Player.Dash.performed -= PlayerDashPerformed;


        playerInputActions.Combat.Attack.started -= PlayerAttack_started;
        playerInputActions.QuickSlots.Slot1.performed -= OnSlot1Performed;
        playerInputActions.QuickSlots.Slot2.performed -= OnSlot2Performed;
        playerInputActions.Player.ToggleInventory.performed -= OnToggleInventoryPerformed;
        playerInputActions.Player.Pause.performed -= Pause_performed;
        playerInputActions.Player.ToggleCharacterMenu.performed -= OnCharacterMenuPerformed;
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }

    public void DisableMovement()
    {
        playerInputActions.Disable();
    }

    private void PlayerAttack_started(InputAction.CallbackContext obj)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }

    private void OnSlot1Performed(InputAction.CallbackContext context)
    {
        OnSlot1Pressed?.Invoke(this, EventArgs.Empty);
    }

    private void OnSlot2Performed(InputAction.CallbackContext context)
    {
        OnSlot2Pressed?.Invoke(this, EventArgs.Empty);
    }

    private void OnToggleInventoryPerformed(InputAction.CallbackContext context)
    {
        OnInventoryToggled?.Invoke(this, EventArgs.Empty);
    }

    private void Pause_performed(InputAction.CallbackContext context)
    {
        OnPausePressed?.Invoke(this, EventArgs.Empty);
    }

    private void OnCharacterMenuPerformed(InputAction.CallbackContext context)
    {
        OnCharacterMenuToggled?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerDashPerformed(InputAction.CallbackContext obj)
    {
        OnPlayerDash?.Invoke(this, EventArgs.Empty);
    }
}
