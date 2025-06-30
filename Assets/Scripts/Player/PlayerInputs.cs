using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction crouchAction;
    public InputAction sprintAction;
    public InputAction interactAction;
    public Vector2 PlayerMouseInput { get; private set; }

    public void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    public void Update()
    {
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
}