using UnityEngine;

public class WalkState : PlayerState
{
    public WalkState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.CurrentSpeed = player.PlayerStats.WalkSpeed;
        StartFootSteps(SoundType.Walk, 0.45f, 0.5f);
    }

    public override void ExitState()
    {
        StopFootSteps();
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