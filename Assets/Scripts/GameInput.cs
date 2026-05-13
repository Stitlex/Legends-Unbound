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

    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.QuickSlots.Enable();

        playerInputActions.Combat.Attack.started += PlayerAttack_started;
        playerInputActions.QuickSlots.Slot1.performed += OnSlot1Performed;
        playerInputActions.QuickSlots.Slot2.performed += OnSlot2Performed;
    }

    private void OnDestroy()
    {
        playerInputActions.Disable();
        playerInputActions.Dispose();

        playerInputActions.Combat.Attack.started -= PlayerAttack_started;
        playerInputActions.QuickSlots.Slot1.performed -= OnSlot1Performed;
        playerInputActions.QuickSlots.Slot2.performed -= OnSlot2Performed;
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
}
