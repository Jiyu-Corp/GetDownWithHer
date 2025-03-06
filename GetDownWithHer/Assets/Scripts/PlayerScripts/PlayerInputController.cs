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
    private InputAction holdHandL;
    private InputAction holdHandR;

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
        holdHandL = playerInput.actions["HoldHandL"];
        holdHandR = playerInput.actions["HoldHandR"];
    }

    private void BindHandlersToActions() {
        jumpAction.performed += ctx => climberEntity.Jump();
        moveAction.started += ctx => climberEntity.StartMove(ctx.ReadValue<Vector2>().x);
        moveAction.canceled += ctx => climberEntity.StopMove();

        moveHandsAction.performed += ctx => climberEntity.SetDirectionPointer(mainCamera.ScreenToWorldPoint(ctx.ReadValue<Vector2>()));

        const int L_HAND = 0;
        const int R_HAND = 1;
        holdHandL.started += ctx => climberEntity.ManageHoldHand(L_HAND, true);
        holdHandL.canceled += ctx => climberEntity.ManageHoldHand(L_HAND, false);
        holdHandR.started += ctx => climberEntity.ManageHoldHand(R_HAND, true);
        holdHandR.canceled += ctx => climberEntity.ManageHoldHand(R_HAND, false);
    } 
}
