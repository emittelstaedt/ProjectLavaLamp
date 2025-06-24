using UnityEngine;

public class WalkState : PlayerState
{
    public WalkState(PlayerController player) : base(player) { }

    public override void Update()
    {
        Vector2 moveValue = player.moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = player.CalculateMoveDirection(moveValue);

        if (moveValue.magnitude < 0.1f)
        {
            player.SwitchState(new IdleState(player));
            return;
        }

        if (player.jumpAction.IsPressed() && player.IsGrounded() && !player.IsCrouching)
        {
            player.SwitchState(new JumpState(player));
            return;
        }
        if (player.sprintAction.IsPressed() && !player.IsOnSprintCooldown)
        {
            player.SwitchState(new SprintState(player));
        }
        player.ApplyGravity();
        player.MovePlayer(moveDirection * player.playerStats.WalkSpeed);
    }
}