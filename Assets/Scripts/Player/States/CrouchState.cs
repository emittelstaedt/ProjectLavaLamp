using UnityEngine;

public class CrouchState : PlayerState
{
    public CrouchState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.IsCrouching = true;
        player.CharacterController.height = player.playerStats.CrouchHeight;
    }

    public override void Update()
    {
        Vector2 moveValue = player.moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = player.CalculateMoveDirection(moveValue);

        if (!player.crouchAction.IsPressed())
        {
            player.SwitchState(moveValue.magnitude > 0.1f ? new WalkState(player) : new IdleState(player));
            return;
        }

        player.ApplyGravity();
        player.MovePlayer(moveDirection * (player.playerStats.CrouchSpeed));
    }
    public override void ExitState()
    {
        player.IsCrouching = false;
        player.CharacterController.height = player.playerStats.NormalHeight;
    }
}