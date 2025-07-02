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

        if (player.IsCrouchButtonPressed)
        {
            player.SwitchState(new CrouchState(player));
        }

        if (player.IsJumpButtonPressed && player.IsGrounded())
        {
            player.SwitchState(new JumpState(player));
            return;
        }
        
        if (player.IsSprintButtonPressed && player.IsMovingForward())
        {
            player.SwitchState(new SprintState(player));
        }
    }
}