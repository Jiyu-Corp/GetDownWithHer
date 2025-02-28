using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour {
    private PlayerInput playerInput;

    // Actions
    private InputAction jumpAction;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        jumpAction =  playerInput.actions["Jump"];
    }
}
