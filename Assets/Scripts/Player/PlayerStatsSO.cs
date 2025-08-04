using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Player/PlayerStatsSO")]
public class PlayerStatsSO : ScriptableObject
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float airControl = 3f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float normalHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTime = 0.1f;
    [SerializeField] private float sprintDuration = 2f;

    public float WalkSpeed => walkSpeed;
    public float SprintSpeed => sprintSpeed;
    public float CrouchSpeed => crouchSpeed;
    public float AirControl => airControl;
    public float JumpForce => jumpForce;
    public float Gravity => gravity;
    public float NormalHeight => normalHeight;
    public float CrouchHeight => crouchHeight;
    public float CrouchTime => crouchTime;
    public float SprintDuration => sprintDuration;
}