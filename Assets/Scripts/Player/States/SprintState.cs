using UnityEngine;

public class SprintState : PlayerState
{
    public SprintState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.CurrentSpeed = player.PlayerStats.SprintSpeed;
        StartFootSteps(SoundType.Sprint, 0.2f, 0.25f);
    }

    public override void ExitState()
    {
        StopFootSteps();
    }

    public override void Update()
    {
        if (!player.IsSprintButtonPressed || !player.IsMovingForward())
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