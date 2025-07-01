using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        Vector3 newVelocity = player.Velocity;
        newVelocity.y = player.PlayerStats.JumpForce;
        player.Velocity = newVelocity;
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