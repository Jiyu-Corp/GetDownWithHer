using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour {
    private PlayerInput playerInput;
    private ClimberEntity climberEntity;

    // Actions
    private InputAction jumpAction;
    private InputAction moveAction;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        climberEntity = GetComponent<ClimberEntity>();

        InitActions();
        BindHandlersToActions();
    }

    private void InitActions() {
        jumpAction = playerInput.actions["Jump"];
        moveAction = playerInput.actions["Move"];
    }

    private void BindHandlersToActions() {
        jumpAction.performed += ctx => climberEntity.Jump();
        moveAction.performed += ctx => climberEntity.Move(ctx.ReadValue<Vector2>().x);
    } 
}
