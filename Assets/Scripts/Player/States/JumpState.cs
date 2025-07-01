using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.YVelocity = player.PlayerStats.JumpForce;
    }

    public override void Update()
    {
        if (player.IsGrounded())
        {
            if (player.IsMoving())
            {
                player.SwitchState(new WalkState(player));
            }
            else
            {
                player.SwitchState(new IdleState(player));
            }
        }
    }
}