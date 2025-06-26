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
        Vector2 moveValue = player.PlayerInputs.moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = player.CalculateMoveDirection(moveValue);

        if (player.IsGrounded() && player.Velocity.y <= 0)
        {
            if (moveValue.magnitude > 0.1f)
            {
                player.SwitchState(new WalkState(player));
            }
            else
            {
                player.SwitchState(new IdleState(player));
            }
        }

        player.ApplyGravity();
        player.MovePlayer(moveDirection * player.PlayerStats.WalkSpeed);
    }
}