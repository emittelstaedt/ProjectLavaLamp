using UnityEngine;

public class CrouchState : PlayerState
{
    public CrouchState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.IsCrouching = true;
        player.TargetHeight = player.PlayerStats.CrouchHeight;

        player.CurrentSpeed = player.PlayerStats.CrouchSpeed;
    }

    public override void Update()
    {
        if (!player.PlayerInputs.crouchAction.IsPressed() && player.HasRoomToUncrouch())
        {
            player.SwitchState(player.IsMoving() ? new WalkState(player) : new IdleState(player));
            return;
        }

        if (player.PlayerInputs.jumpAction.IsPressed() && player.IsGrounded() && player.HasRoomToUncrouch())
        {
            player.SwitchState(new JumpState(player));
        }
    }

    public override void ExitState()
    {
        player.IsCrouching = false;
        player.TargetHeight = player.PlayerStats.NormalHeight;
    }
}