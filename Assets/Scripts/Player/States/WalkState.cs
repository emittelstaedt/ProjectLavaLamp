using UnityEngine;

public class WalkState : PlayerState
{
    public WalkState(PlayerController player) : base(player) { }

    public override void Update()
    {
        Vector2 moveValue = player.PlayerInputs.moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = player.CalculateMoveDirection(moveValue);

        if (moveValue.magnitude < 0.1f)
        {
            player.SwitchState(new IdleState(player));
            return;
        }

        if (player.PlayerInputs.crouchAction.IsPressed())
        {
            player.SwitchState(new CrouchState(player));
        }

        if (player.PlayerInputs.jumpAction.IsPressed() && player.IsGrounded() && !player.IsCrouching)
        {
            player.SwitchState(new JumpState(player));
            return;
        }
        if (player.PlayerInputs.sprintAction.IsPressed() && !player.IsOnSprintCooldown)
        {
            player.SwitchState(new SprintState(player));
        }
        player.ApplyGravity();
        player.MovePlayer(moveDirection * player.PlayerStats.WalkSpeed);
    }
}
