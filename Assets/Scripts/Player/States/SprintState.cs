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
        if (!player.IsSprintButtonPressed || player.IsOnSprintCooldown())
        {
            player.SwitchState(new WalkState(player));
            return;
        }

        if (player.IsCrouchButtonPressed)
        {
            player.SwitchState(new CrouchState(player));
        }

        if (player.IsJumpButtonPressed && player.IsGrounded())
        {
            player.SwitchState(new JumpState(player));
            return;
        }
    }
}