using UnityEngine;

public class CrouchState : PlayerState
{
    public CrouchState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.IsCrouching = false;

        player.CharacterController.enabled = false;

        Vector3 newHeight = new Vector3(1, player.PlayerStats.CrouchHeight, 1);
        Vector3 originalHeight = player.transform.localScale;
        float difference = player.PlayerStats.CrouchHeight - player.PlayerStats.NormalHeight;
        player.transform.localScale = newHeight;

        player.transform.position += Vector3.up * difference;

        player.CharacterController.enabled = true;
    }

    public override void Update()
    {

        Vector2 moveValue = player.PlayerInputs.sprintAction.ReadValue<Vector2>();
        Vector3 moveDirection = player.CalculateMoveDirection(moveValue);

        if (!player.PlayerInputs.crouchAction.IsPressed())
        {
            player.SwitchState(moveValue.magnitude > 0.1f ? new WalkState(player) : new IdleState(player));
            return;
        }
        
        if (player.PlayerInputs.jumpAction.IsPressed() && player.IsGrounded())
        {
            player.SwitchState(new JumpState(player));
        }

        player.ApplyGravity();
        player.MovePlayer(moveDirection * (player.PlayerStats.CrouchSpeed));
    }

    public override void ExitState()
    {
        player.IsCrouching = false;

        player.CharacterController.enabled = false;

        Vector3 newHeight = new Vector3(1, player.PlayerStats.NormalHeight, 1);
        Vector3 originalHeight = player.transform.localScale;
        float difference = player.PlayerStats.NormalHeight - player.PlayerStats.CrouchHeight;
        player.transform.localScale = newHeight;

        player.transform.position += Vector3.up * difference;

        player.CharacterController.enabled = true;
    }
}