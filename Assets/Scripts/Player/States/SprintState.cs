using UnityEngine;

public class SprintState : PlayerState
{
    public SprintState(PlayerController player) : base(player) { }

    public override void Update()
    {
        Vector2 moveValue = player.PlayerInputs.moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = player.CalculateMoveDirection(moveValue);

        if (!player.PlayerInputs.sprintAction.IsPressed() || player.IsCrouching || moveValue.magnitude < 0.1f || player.IsOnSprintCooldown)
        {
            player.SwitchState(new WalkState(player));
            return;
        }

        if (player.PlayerInputs.crouchAction.IsPressed())
        {
            player.SwitchState(new CrouchState(player));
        }

        if (player.PlayerInputs.jumpAction.IsPressed() && player.IsGrounded())
        {
            player.SwitchState(new JumpState(player));
            return;
        }

        player.SprintTimer -= Time.deltaTime;
        if (player.SprintTimer <= 0f)
        {
            player.IsOnSprintCooldown = true;
            player.SprintCooldownTimer = player.PlayerStats.SprintCooldownDuration;
            player.SwitchState(new WalkState(player));
        }

        player.ApplyGravity();
        player.MovePlayer(moveDirection * player.PlayerStats.SprintSpeed);
    }
}