using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Update()
    {
        if (player.IsMoving())
        {
            player.SwitchState(new WalkState(player));
        }

        if (player.IsJumpButtonPressed && player.IsGrounded())
        {
            player.SwitchState(new JumpState(player));
        }

        if (player.IsCrouchButtonPressed)
        {
            player.SwitchState(new CrouchState(player));
        }
    }
}