using UnityEngine;

public class WalkState : PlayerState
{
    public WalkState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.CurrentSpeed = player.PlayerStats.WalkSpeed;
    }

    public override void Update()
    {
        if (!player.IsMoving())
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
    }
}