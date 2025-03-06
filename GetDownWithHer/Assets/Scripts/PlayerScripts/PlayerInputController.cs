using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour {
    public Camera mainCamera;

    private PlayerInput playerInput;
    private ClimberEntity climberEntity;

    // Actions
    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction moveHandsAction;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        climberEntity = GetComponent<ClimberEntity>();

        InitActions();
        BindHandlersToActions();
    }

    private void InitActions() {
        jumpAction = playerInput.actions["Jump"];
        moveAction = playerInput.actions["Move"];
        moveHandsAction = playerInput.actions["MoveHands"];
    }

    private void BindHandlersToActions() {
        jumpAction.performed += ctx => climberEntity.Jump();
        moveAction.started += ctx => climberEntity.StartMove(ctx.ReadValue<Vector2>().x);
        moveAction.canceled += ctx => climberEntity.StopMove();
        moveHandsAction.performed += ctx => climberEntity.SetDirectionPointer(mainCamera.ScreenToWorldPoint(ctx.ReadValue<Vector2>()));
    } 
}
