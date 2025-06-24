using UnityEngine;

public class SprintState : PlayerState
{
    public SprintState(PlayerController player) : base(player) { }

    public override void Update()
    {
        Vector2 moveValue = player.moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = player.CalculateMoveDirection(moveValue);

        if (!player.sprintAction.IsPressed() || player.IsCrouching || moveValue.magnitude < 0.1f || player.IsOnSprintCooldown)
        {
            player.SwitchState(new WalkState(player));
            return;
        }

        if (player.jumpAction.IsPressed() && player.IsGrounded())
        {
            player.SwitchState(new JumpState(player));
            return;
        }
        player.ApplyGravity();
        player.MovePlayer(moveDirection * player.playerStats.SprintSpeed);

        player.SprintTimer -= Time.deltaTime;
        if (player.SprintTimer <= 0f)
        {
            player.IsOnSprintCooldown = true;
            player.SprintCooldownTimer = player.playerStats.SprintCooldownDuration;
            player.SwitchState(new WalkState(player));
        }
    }
}