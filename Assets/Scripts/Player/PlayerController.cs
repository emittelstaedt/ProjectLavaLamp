using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Camera and player inputs
    [SerializeField] private Transform playerCamera;
    public CharacterController CharacterController;
    private Vector2 playerMouseInput;
    private float xRotation;

    //Physics variables
    private Vector3 velocity;

    //Crouching
    private bool isCrouching = false;

    //Sprinting
    private float sprintTimer;
    private float sprintCooldownTimer;
    private bool isOnSprintCooldown = false;

    //Implementing getters and setters
    public Vector3 Velocity
    {
        get => velocity;
        set => velocity = value;
    }
    public bool IsCrouching
    {
        get => isCrouching;
        set => isCrouching = value;
    }

    public bool IsOnSprintCooldown
    {
        get => isOnSprintCooldown;
        set => isOnSprintCooldown = value;
    }

    public float SprintTimer
    {
        get => sprintTimer;
        set => sprintTimer = value;
    }
    public float SprintCooldownTimer
    {
        get => sprintCooldownTimer;
        set => sprintCooldownTimer = value;
    }


    //Input Actions
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction crouchAction;
    public InputAction sprintAction;

    private PlayerState currentState;
    public PlayerStatsSO playerStats;
    private void Start()
    {
        CharacterController = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        sprintAction = InputSystem.actions.FindAction("Sprint");

        currentState = new IdleState(this);
        currentState.EnterState();
    }
    private void Update()
    {
        playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        currentState.Update();
        MovePlayerCamera();

        //Check for if enough time has passed to reset the sprint cooldown and sprint timer
        if (isOnSprintCooldown)
        {
            sprintCooldownTimer -= Time.deltaTime;
            if (sprintCooldownTimer <= 0f)
            {
                isOnSprintCooldown = false;
            }
        }


        if (!isOnSprintCooldown && !(currentState is SprintState) && sprintTimer < playerStats.SprintDuration)
        {
            sprintTimer += Time.deltaTime;
            sprintTimer = Mathf.Min(sprintTimer, playerStats.SprintDuration);
        }
    }

    public void SwitchState(PlayerState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public Vector3 CalculateMoveDirection(Vector2 moveValue)
    {
        Vector3 direction = (transform.right * moveValue.x + transform.forward * moveValue.y).normalized;
        return direction;
    }

    public void MovePlayer(Vector3 horizontal)
    {
        Vector3 finalMove = horizontal + new Vector3(0, velocity.y, 0);
        CharacterController.Move(finalMove * Time.deltaTime);
    }

    public void ApplyGravity()
    {
        if(IsGrounded() && velocity.y < 0)
        {
            velocity.y = -1f;
        }
        else
        {
            velocity.y -= playerStats.Gravity * -2f * Time.deltaTime;
        }
    }

    public bool IsGrounded()
    {
        return CharacterController.isGrounded;
    }
    private void MovePlayerCamera()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            xRotation -= playerMouseInput.y * playerStats.Sensitivity;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.Rotate(0f, playerMouseInput.x * playerStats.Sensitivity, 0f);
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}