using UnityEngine;

public class CrouchState : PlayerState
{
    public CrouchState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.IsCrouching = true;
        SetHeight(player.PlayerStats.CrouchHeight);
    }

    public override void Update()
    {

        Vector2 moveValue = player.PlayerInputs.moveAction.ReadValue<Vector2>();
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
        SetHeight(player.PlayerStats.NormalHeight);
    }

    private void SetHeight(float newHeight)
    {
        // Disables CharacterController temporarily to prevent its internal variables overriding our changes
        player.CharacterController.enabled = false;

        player.transform.localScale = new Vector3(player.transform.localScale.x, newHeight, player.transform.localScale.z);

        float difference = player.transform.localScale.y - newHeight;
        player.transform.position += Vector3.up * difference;

        player.CharacterController.enabled = true;
    }
}