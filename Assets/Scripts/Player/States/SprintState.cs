using UnityEngine;

public class SprintState : PlayerState
{
    public SprintState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.CurrentSpeed = player.PlayerStats.SprintSpeed;
    }

    public override void Update()
    {
        if (!player.PlayerInputs.sprintAction.IsPressed() || player.IsOnSprintCooldown())
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
    }
}