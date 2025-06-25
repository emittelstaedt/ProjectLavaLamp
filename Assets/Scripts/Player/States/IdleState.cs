using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }
    public override void Update()
    {
        Vector2 move = player.PlayerInputs.moveAction.ReadValue<Vector2>();
        if(move.magnitude > 0.1f)
        {
            player.SwitchState(new WalkState(player));
        }

        if(player.PlayerInputs.jumpAction.IsPressed() && player.IsGrounded())
        {
            player.SwitchState(new JumpState(player));
        }

        if (player.PlayerInputs.crouchAction.IsPressed())
        {
            player.SwitchState(new CrouchState(player));
        }

        player.ApplyGravity();
        player.MovePlayer(Vector3.zero);
    }
}
