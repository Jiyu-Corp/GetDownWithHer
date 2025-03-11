using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour {
    public Camera mainCamera;

    private PlayerInput playerInput;
    private Entity entity;

    // Actions
    private InputAction jumpAction;
    //private InputAction moveAction; This was removed cause multiple bind call to same action was returning wrong value, on the canceled event
    private InputAction moveRightAction;
    private InputAction moveLeftAction;
    private InputAction holdHandLAction;
    private InputAction holdHandRAction;
    private InputAction annoyPrincessAction;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        entity = GetComponent<Entity>();

        // InputSystem.settings.SetInternalFeatureFlag("DISABLE_SHORTCUT_SUPPORT", true); This was used to fix multiple bind call in vector... but didnt solve

        InitActions();
        BindHandlersToActions();
    }

    private void InitActions() {
        jumpAction = playerInput.actions["Jump"];
        //moveAction = playerInput.actions["Move"];
        moveRightAction = playerInput.actions["MoveRight"];
        moveLeftAction = playerInput.actions["MoveLeft"];
        holdHandLAction = playerInput.actions["HoldHandL"];
        holdHandRAction = playerInput.actions["HoldHandR"];
        annoyPrincessAction = playerInput.actions["AnnoyPrincess"];
    }

    private void BindHandlersToActions() {
        jumpAction.performed += ctx => entity.Jump();
        moveRightAction.started += ctx => entity.ComposeMoveDirection(1f, true);
        moveRightAction.canceled += ctx => entity.ComposeMoveDirection(1f, false);
        moveLeftAction.started += ctx => entity.ComposeMoveDirection(-1f, true);
        moveLeftAction.canceled += ctx => entity.ComposeMoveDirection(-1f, false);

        ClimberEntity cEntity = entity as ClimberEntity;
        if(cEntity != null) {
            const int L_HAND = 0;
            const int R_HAND = 1;
            holdHandLAction.started += ctx => cEntity.ManageHoldHand(L_HAND, true);
            holdHandLAction.canceled += ctx => cEntity.ManageHoldHand(L_HAND, false);
            holdHandRAction.started += ctx => cEntity.ManageHoldHand(R_HAND, true);
            holdHandRAction.canceled += ctx => cEntity.ManageHoldHand(R_HAND, false);
        }

        Player player = entity as Player;
        if(player != null) {
            annoyPrincessAction.started += ctx => player.StartAnnoyPrincess();
            annoyPrincessAction.canceled += ctx => player.StopAnnoyPrincess();
        }
    }

    private void FixedUpdate() {
        UpdatePlayerDirectionPointer();
    }

    private void UpdatePlayerDirectionPointer() {
        ClimberEntity cEntity = entity as ClimberEntity;
        if(cEntity == null) return;

        Vector2 newPointerPositionOnWorldSpace = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        cEntity.SetDirectionPointer(newPointerPositionOnWorldSpace);
    }
}
