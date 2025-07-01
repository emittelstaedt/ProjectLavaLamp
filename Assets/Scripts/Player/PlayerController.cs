using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerInputs playerInputs;
    [SerializeField] private PlayerCameraControls playerCameraControls;
    [SerializeField] private PlayerStatsSO playerStats;
    private PlayerState currentState;
    private float crouchVelocity;
    private float sprintTimer;
    private float sprintCooldownTimer;
    private float yVelocity;
    private float targetHeight;
    private float currentSpeed;

    public PlayerStatsSO PlayerStats => playerStats;
    public PlayerInputs PlayerInputs => playerInputs;
    public float YVelocity
    {
        get => yVelocity;
        set => yVelocity = value;
    }
    public float TargetHeight
    {
        get => targetHeight;
        set => targetHeight = Mathf.Max(value, 1);
    }
    public float CurrentSpeed
    {
        get => currentSpeed;
        set => currentSpeed = value;
    }

    private void Awake()
    {
        currentState = new IdleState(this);
        currentState.EnterState();
        
        targetHeight = playerStats.NormalHeight;
    }

    private void Update()
    {
        playerCameraControls.MovePlayerCamera();
        playerCameraControls.LockCursor();

        ManageSprintTimers();
        UpdateHeight();
        MovePlayer();

        currentState.Update();
    }

    public void SwitchState(PlayerState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
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

    public bool IsOnSprintCooldown()
    {
        return sprintCooldownTimer > 0f;
    }

    private void ManageSprintTimers()
    {
        if (IsOnSprintCooldown())
        {
            sprintCooldownTimer -= Time.deltaTime;
        }
        else if (currentState is not SprintState)
        {
            sprintTimer += Time.deltaTime;
            sprintTimer = Mathf.Min(sprintTimer, playerStats.SprintDuration);
        }
        else
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0f)
            {
                sprintCooldownTimer = playerStats.SprintCooldownDuration;
            }
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

    private void MovePlayer()
    {
        Vector2 input = PlayerInputs.moveAction.ReadValue<Vector2>();
        Vector3 inputDirection = (transform.right * input.x + transform.forward * input.y).normalized;

        ApplyGravity();

        Vector3 movement = (inputDirection * currentSpeed) + (Vector3.up * yVelocity);
        characterController.Move(movement * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && yVelocity < 0)
        {
            yVelocity = -1f;
        }
        else
        {
            yVelocity += playerStats.Gravity * Time.deltaTime;
        }
    }
}
