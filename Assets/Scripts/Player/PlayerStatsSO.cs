using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Player/PlayerStatsSO")]
public class PlayerStatsSO : ScriptableObject
{
    private float walkSpeed = 5f;
    private float sprintSpeed = 10f;
    private float crouchSpeed = 3f;

    private float jumpForce = 10f;
    private float gravity = -9.81f;
    private float normalHeight = 2f;
    private float crouchHeight = 1f;
    private float sprintDuration = 2f;
    private float sprintCooldownDuration = 2f;
    private float sensitivity = 7f;

    // Public getters (read-only)
    public float WalkSpeed => walkSpeed;
    public float SprintSpeed => sprintSpeed;
    public float CrouchSpeed => crouchSpeed;
    public float JumpForce => jumpForce;
    public float Gravity => gravity;
    public float NormalHeight => normalHeight;
    public float CrouchHeight => crouchHeight;
    public float SprintDuration => sprintDuration;
    public float SprintCooldownDuration => sprintCooldownDuration;
    public float Sensitivity => sensitivity;
}
