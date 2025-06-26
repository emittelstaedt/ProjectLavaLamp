using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Player/PlayerStatsSO")]
public class PlayerStatsSO : ScriptableObject
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float normalHeight = 1.5f;
    [SerializeField] private float crouchHeight = 0.7f;
    [SerializeField] private float sprintDuration = 2f;
    [SerializeField] private float sprintCooldownDuration = 2f;

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
}