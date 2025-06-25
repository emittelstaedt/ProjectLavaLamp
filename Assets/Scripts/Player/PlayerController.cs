using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Camera and player inputs
    [SerializeField] private CharacterController characterController;
    public CharacterController CharacterController => characterController;
    [SerializeField] private PlayerInputs playerInputs;
    [SerializeField] private PlayerCameraControls playerCameraControls;
    public PlayerCameraControls PlayerCameraControls => playerCameraControls;
    [SerializeField] private PlayerState currentState;
    [SerializeField] private PlayerStatsSO playerStats;
    public PlayerStatsSO PlayerStats => playerStats;
    public PlayerInputs PlayerInputs => playerInputs;
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
    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        currentState = new IdleState(this);
        currentState.EnterState();
    }
    private void Update()
    {
        playerCameraControls.MovePlayerCamera();
        playerCameraControls.LockCursor();
        currentState.Update();

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
        characterController.Move(finalMove * Time.deltaTime);
    }

    public void ApplyGravity()
    {
        if(IsGrounded() && velocity.y < 0)
        {
            velocity.y = -1f;
        }
        else
        {
            velocity.y += playerStats.Gravity * Time.deltaTime;
        }
    }

    public bool IsGrounded()
    {
        return characterController.isGrounded;
    }
}