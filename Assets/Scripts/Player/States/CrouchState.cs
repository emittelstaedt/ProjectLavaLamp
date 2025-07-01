using UnityEngine;

public class CrouchState : PlayerState
{
    public CrouchState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.TargetHeight = player.PlayerStats.CrouchHeight;

        player.CurrentSpeed = player.PlayerStats.CrouchSpeed;
    }

    public override void Update()
    {
        if (!player.IsCrouchButtonPressed && player.HasRoomToUncrouch())
        {
            player.SwitchState(player.IsMoving() ? new WalkState(player) : new IdleState(player));
            return;
        }

        if (player.IsJumpButtonPressed && player.IsGrounded() && player.HasRoomToUncrouch())
        {
            player.SwitchState(new JumpState(player));
        }
    }

    public override void ExitState()
    {
        player.TargetHeight = player.PlayerStats.NormalHeight;
    }
}