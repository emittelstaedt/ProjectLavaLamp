using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerInputs playerInputs;
    [SerializeField] private PlayerCameraControls playerCameraControls;
    [SerializeField] private PlayerStatsSO playerStats;
    private PlayerState currentState;
    private Vector3 velocity;
    private bool isCrouching = false;
    private float crouchVelocity;
    private float targetHeight;
    private float currentSpeed;
    private float sprintTimer;
    private float sprintCooldownTimer;
    private bool isOnSprintCooldown = false;

    public CharacterController CharacterController => characterController;
    public PlayerCameraControls PlayerCameraControls => playerCameraControls;
    public PlayerStatsSO PlayerStats => playerStats;
    public PlayerInputs PlayerInputs => playerInputs;
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
    public float TargetHeight
    {
        get => targetHeight;
        set => targetHeight = value;
    }
    public float CurrentSpeed
    {
        get => currentSpeed;
        set => currentSpeed = value;
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

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        currentState = new IdleState(this);
        currentState.EnterState();
        
        targetHeight = playerStats.NormalHeight;
    }

    private void Update()
    {
        playerCameraControls.MovePlayerCamera();
        playerCameraControls.LockCursor();
        currentState.Update();
        UpdateHeight();
        MovePlayer();

        // Check for if enough time has passed to reset the sprint cooldown and sprint timer
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
    
    private void UpdateHeight()
    {
        // Disables CharacterController temporarily to prevent its internal variables overriding our changes.
        characterController.enabled = false;

        float newHeight = Mathf.SmoothDamp(characterController.height, targetHeight, ref crouchVelocity, playerStats.CrouchTime);

        // Height adjustment to keep player grounded.
        float heightDifference = newHeight - characterController.height;        
        float worldHeightDifference = heightDifference * transform.localScale.y;
        
        characterController.height = newHeight;
        
        // Player scales from its center, so only adjust half the world height difference.
        transform.position += Vector3.up * worldHeightDifference / 2;

        characterController.enabled = true;
    }

    public void SwitchState(PlayerState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    private void MovePlayer()
    {
        Vector2 input = PlayerInputs.moveAction.ReadValue<Vector2>();
        Vector3 inputDirection = (transform.right * input.x + transform.forward * input.y).normalized;

        ApplyGravity();

        Vector3 movement = (inputDirection * currentSpeed) + Vector3.up * velocity.y;
        characterController.Move(movement * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && velocity.y < 0)
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

    public bool IsMoving()
    {
        return playerInputs.moveAction.ReadValue<Vector2>().magnitude > 0.1f;
    }

    public bool HasRoomToUncrouch()
    {
        float radius = characterController.radius;

        float distanceToBottomSphere = (characterController.height / 2) - radius;
        float worldDistanceToBottomSphere = distanceToBottomSphere * transform.localScale.y;

        float distanceToTopSphere = playerStats.NormalHeight - (characterController.height / 2) - radius;
        float worldDistanceToTopSphere = distanceToTopSphere * transform.localScale.y;

        Vector3 bottomSphereCenter = transform.position - Vector3.up * worldDistanceToBottomSphere;
        Vector3 topSphereCenter = transform.position + Vector3.up * worldDistanceToTopSphere;

        return !Physics.CheckCapsule(bottomSphereCenter, topSphereCenter, radius);
    }
}
