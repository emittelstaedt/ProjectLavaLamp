using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }
    public override void Update()
    {
        Vector2 move = player.moveAction.ReadValue<Vector2>();
        if(move.magnitude > 0.1f)
        {
            player.SwitchState(new WalkState(player));
        }

        if(player.jumpAction.IsPressed() && player.IsGrounded() && !player.IsCrouching)
        {
            player.SwitchState(new JumpState(player));
        }

        if (player.crouchAction.IsPressed())
        {
            player.SwitchState(new CrouchState(player));
        }

        player.ApplyGravity();
        player.MovePlayer(Vector3.zero);
    }
}
