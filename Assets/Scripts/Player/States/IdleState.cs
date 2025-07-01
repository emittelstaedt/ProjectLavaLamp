using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.CurrentSpeed = 0f;
    }

    public override void Update()
    {
        if (player.IsMoving())
        {
            player.SwitchState(new WalkState(player));
        }

        if (player.PlayerInputs.jumpAction.IsPressed() && player.IsGrounded())
        {
            player.SwitchState(new JumpState(player));
        }

        if (player.PlayerInputs.crouchAction.IsPressed())
        {
            player.SwitchState(new CrouchState(player));
        }
    }
}